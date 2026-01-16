using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Pagination;
using axionpro.application.Interfaces;
using axionpro.application.Interfaces.IFileStorage;

using axionpro.application.Interfaces.IQRService;
using axionpro.application.Interfaces.IRepositories;
using axionpro.domain.Entity;
using axionpro.persistance.Data.Context;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace axionpro.persistance.Repositories
{
    
 

    public class AssetRepository : IAssetRepository
    {
        private WorkforceDbContext _context;

        private ILogger<AssetRepository> _logger;
        private readonly IQRService _qrService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDbContextFactory<WorkforceDbContext> _contextFactory;
        private readonly IMapper _mapper;


        public AssetRepository(WorkforceDbContext context, ILogger<AssetRepository> logger, IQRService qrService, IFileStorageService fileStorageService,
            IMapper mapper, IDbContextFactory<WorkforceDbContext> contextFactory)
        {
            _context = context;
            _logger = logger;
            _qrService = qrService;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            _contextFactory = contextFactory;
        }

        public async Task<GetAssetResponseDTO> UpdateAsync(
     Asset asset,
     string? path)
        {
            try
            {
                _logger.LogInformation(
                    "Updating asset Id={AssetId}", asset.Id);

                // ============================
                // 1️⃣ Fetch Existing Asset
                // ============================
                var existingAsset = await _context.Assets
                    .FirstOrDefaultAsync(a =>
                        a.Id == asset.Id &&
                        a.IsSoftDeleted != true);

                if (existingAsset == null)
                    throw new Exception("Asset not found.");

                // ============================
                // 2️⃣ Update Asset Fields
                // (Already NULL-safe from handler)
                // ============================
                _context.Entry(existingAsset)
                    .CurrentValues
                    .SetValues(asset);

                // ============================
                // 3️⃣ IMAGE UPDATE (SINGLE IMAGE RULE)
                // ============================
                var assetImage = await GetAssetImageAsync(existingAsset.Id);

                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (assetImage != null)
                    {
                        // 🔁 Update same image
                        assetImage.AssetImagePath = path;
                        assetImage.IsActive = existingAsset.IsActive;
                        assetImage.UpdatedById = asset.UpdatedById;
                        assetImage.UpdatedDateTime = DateTime.UtcNow;
                    }
                    else
                    {
                        // ⚠️ Edge case: image missing → create once
                        assetImage = new AssetImage
                        {
                            AssetId = existingAsset.Id,
                            TenantId = existingAsset.TenantId,
                            AssetImageType = ConstantValues.Web,
                            AssetImagePath = path,
                            IsPrimary = true,
                            IsActive = existingAsset.IsActive,
                            IsSoftDeleted = false,
                            AddedById = asset.UpdatedById,
                            AddedDateTime = DateTime.UtcNow
                        };

                        await _context.AssetImages.AddAsync(assetImage);
                    }
                }

                // ============================
                // 4️⃣ Sync Image with Asset State
                // ============================
                if (assetImage != null)
                {
                    assetImage.IsActive = existingAsset.IsActive;

                    if (existingAsset.IsSoftDeleted == true)
                    {
                        assetImage.IsSoftDeleted = true;
                    }
                }

                // ============================
                // 5️⃣ SAVE
                // ============================
                await _context.SaveChangesAsync();

                // ============================
                // 6️⃣ RETURN DTO
                // ============================
                return MapToAssetDTO(existingAsset, assetImage);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while updating asset Id={AssetId}",
                    asset.Id);

                throw;
            }
        }
        #region 🔹 Common Helpers

    

        private async Task<AssetImage?> GetAssetImageAsync(long assetId)
        {
            return await _context.AssetImages
                .FirstOrDefaultAsync(i =>
                    i.AssetId == assetId &&
                    i.IsSoftDeleted != true);
        }

        private static GetAssetResponseDTO MapToAssetDTO(
            Asset asset,
            AssetImage? image = null)
        {
            return new GetAssetResponseDTO
            {
                AssetId = asset.Id,
                AssetName = asset.AssetName,
                AssetTypeId = asset.AssetTypeId,
                Company = asset.Company,
                ModelNo = asset.ModelNo,
                Size = asset.Size,
                Weight = asset.Weight,
                Color = asset.Color,
                IsRepairable = asset.IsRepairable,
                Price = asset.Price,
                SerialNumber = asset.SerialNumber,
                Barcode = asset.Barcode,
                Qrcode = asset.Qrcode,
                PurchaseDate = asset.PurchaseDate,
                WarrantyExpiryDate = asset.WarrantyExpiryDate,
                AssetStatusId = asset.AssetStatusId,
                IsAssigned = asset.IsAssigned ?? false,
                IsActive = asset.IsActive,
                AssetImageId = image?.Id,
                AssetImagePath = image?.AssetImagePath,
                AssetImageType = image?.AssetImageType
            };
        }
        public async Task UpdateQrCodeAsync(long assetId, string qrCode)
        {
            var asset = await _context.Assets
                .FirstOrDefaultAsync(a =>
                    a.Id == assetId &&
                    a.IsSoftDeleted != true);

            if (asset == null)
                throw new Exception("Asset not found for QR update.");

            asset.Qrcode = qrCode;
            await _context.SaveChangesAsync();
        }


        public async Task<GetAssetResponseDTO> AddAsync(Asset asset, string path)
        {
            try
            {
                _logger.LogInformation("Adding new asset for TenantId={TenantId}", asset.TenantId);

                // 1️⃣ Insert Parent Asset
                await _context.Assets.AddAsync(asset);
                await _context.SaveChangesAsync();

                // 2️⃣ Insert Image
                var assetImage = new AssetImage
                {
                    AssetId = asset.Id,
                    TenantId = asset.TenantId,
                    AssetImageType = ConstantValues.Web,
                    AssetImagePath = path,
                    IsPrimary = true,
                    IsActive = true,
                    IsSoftDeleted = false,
                    AddedById = asset.AddedById,
                    AddedDateTime = DateTime.UtcNow
                };

                await _context.AssetImages.AddAsync(assetImage);
                await _context.SaveChangesAsync();

                // 3️⃣ Return DTO
                return MapToAssetDTO(asset, assetImage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ AddAsync failed");
                throw;
            }
        }


        public async Task<List<GetAssetResponseDTO>> GetAllAsync(long tenantId, bool isActive)
        {
            try
            {
                _logger.LogInformation("Fetching assets for TenantId={TenantId}", tenantId);

                var assets = await _context.Assets
                    .Where(a => a.TenantId == tenantId &&
                                a.IsActive == isActive &&
                                a.IsSoftDeleted != true)
                    .OrderByDescending(a => a.Id)
                    .Take(10)
                    .ToListAsync();

                var response = new List<GetAssetResponseDTO>();

                foreach (var a in assets)
                {
                    var img = await GetAssetImageAsync(a.Id);
                    response.Add(MapToAssetDTO(a, img));
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetAllAsync failed");
                throw;
            }
        }

        public async Task<Asset?> GetSingleRecordAsync(long id, bool? isActive)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching single asset | Id={Id} | IsActive={IsActive}",
                    id, isActive);

                IQueryable<Asset> query =
                    _context.Assets.Where(a =>
                        a.Id == id &&  a.IsSoftDeleted != true);

                if (isActive.HasValue)
                    query = query.Where(a => a.IsActive == isActive.Value);

                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetSingleRecordAsync failed for AssetId={Id}", id);
                throw;
            }
        }



        public async Task<List<GetAssetResponseDTO>> GetInsertedAssetAsync(long tenantId, bool isActive)
        {
            try
            {
                var assets = await _context.Assets
                    .Where(a =>
                        a.TenantId == tenantId &&
                        a.IsActive == isActive &&
                        a.IsSoftDeleted != true)
                    .OrderByDescending(a => a.Id)
                    .Take(10)
                    .ToListAsync();

                var result = new List<GetAssetResponseDTO>();

                foreach (var a in assets)
                {
                    var img = await GetAssetImageAsync(a.Id);
                    result.Add(MapToAssetDTO(a, img));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetInsertedAssetAsync failed");
                throw;
            }
        }

        public async Task<List<GetAssetResponseDTO>> GetAssetsByFilterAsync(GetAssetRequestDTO asset)
        {
            try
            {
                if (asset == null || asset.Prop == null)
                    throw new ArgumentException("Invalid filter");

                IQueryable<Asset> query = _context.Assets
                    .Where(a => a.TenantId == asset.Prop.TenantId &&
                                a.IsSoftDeleted != true);

                if (asset.AssetId > 0)
                    query = query.Where(a => a.Id == asset.AssetId);

                if (asset.AssetTypeId > 0)
                    query = query.Where(a => a.AssetTypeId == asset.AssetTypeId);

                if (!string.IsNullOrWhiteSpace(asset.SerialNumber))
                    query = query.Where(a => a.SerialNumber.Contains(asset.SerialNumber));

                if (!string.IsNullOrWhiteSpace(asset.ModelNumber))
                    query = query.Where(a => a.ModelNo.Contains(asset.ModelNumber));

                if (asset.IsActive.HasValue)
                    query = query.Where(a => a.IsActive == asset.IsActive);

                if (asset.IsAssigned.HasValue)
                    query = query.Where(a => a.IsAssigned == asset.IsAssigned);

                var results = await query
                    .OrderByDescending(a => a.Id)
                    .ToListAsync();

                var list = new List<GetAssetResponseDTO>();

                foreach (var a in results)
                {
                    var img = await GetAssetImageAsync(a.Id);
                    list.Add(MapToAssetDTO(a, img));
                }

                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ GetAssetsByFilterAsync failed");
                throw;
            }
        }

        public async Task<bool> DeleteAssetAsync(DeleteAssetReqestDTO asset)
        {
            try
            {
                // 1️⃣ Fetch Existing Asset Entity
                var existingAsset = await _context.Assets
                    .FirstOrDefaultAsync(a =>
                        a.Id == asset.Id &&
                        a.IsSoftDeleted != true);

                if (existingAsset == null)
                {
                    _logger.LogWarning("⚠ Asset not found for Id={Id}", asset.Id);
                    return false;
                }

                // 2️⃣ Soft Delete Asset (ENTITY)
                existingAsset.IsSoftDeleted = true;
                existingAsset.IsActive = false;
                existingAsset.SoftDeletedById = asset.Prop.UserEmployeeId;
                existingAsset.DeletedDateTime = DateTime.UtcNow;

                // 3️⃣ Fetch ALL Asset Images and Soft Delete
                var assetImages = await _context.AssetImages
                    .Where(i => i.AssetId == asset.Id && i.IsSoftDeleted != true)
                    .ToListAsync();

                foreach (var img in assetImages)
                {
                    img.IsSoftDeleted = true;
                    img.IsActive = false;
                    img.SoftDeletedById = asset.Prop.UserEmployeeId;
                    img.DeletedDateTime = DateTime.UtcNow;
                }

                // 4️⃣ Save All at Once
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Asset & Image soft-delete completed | AssetId={Id}", asset.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ DeleteAssetAsync failed");
                return false;
            }
        }


        #endregion





    }
}



