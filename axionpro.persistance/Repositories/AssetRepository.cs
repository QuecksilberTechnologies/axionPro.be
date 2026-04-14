using AutoMapper;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.Interfaces.IFileStorage;

using axionpro.application.Interfaces.IQRService;
using axionpro.application.Interfaces.IRepositories;

using axionpro.persistance.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data; using axionpro.domain.Entity;

namespace axionpro.persistance.Repositories
{
    
 

    public class AssetRepository : IAssetRepository
    {
        private WorkforceDbContext _context;

        private ILogger<AssetRepository> _logger;
        private readonly IQRService _qrService;
        private readonly IFileStorageService _fileStorageService;
       
        private readonly IMapper _mapper;


        public AssetRepository(WorkforceDbContext context, ILogger<AssetRepository> logger, IQRService qrService, IFileStorageService fileStorageService,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _qrService = qrService;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
            
        }

        public async Task<GetAssetResponseDTO> UpdateAsync(Asset asset,string? path)
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
                QRCode = asset.QRCode,
                PurchaseDate = asset.PurchaseDate,
                WarrantyExpiryDate = asset.WarrantyExpiryDate,
                AssetStatusId = asset.AssetStatusId,
                IsAssigned = asset.IsAssigned ,
                IsActive = asset.IsActive,
                HasMultipleUser = asset.AssetType != null && asset.AssetType.AssetCategory != null ? asset.AssetType.AssetCategory.HasMultipleUser : false,
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

            asset.QRCode = qrCode;
            await _context.SaveChangesAsync();
        }


        public async Task<GetAssetResponseDTO> AddAsync(Asset asset, string path)
        {
            try
            {
                _logger.LogInformation("Adding new asset for TenantId={TenantId}", asset.TenantId);

                // 🔥 Attach child entity using navigation property
                if (!string.IsNullOrEmpty(path))
                {
                    asset.AssetImages.Add(new AssetImage
                    {
                        TenantId = asset.TenantId,
                        AssetImageType = ConstantValues.Web,
                        AssetImagePath = path,
                        IsPrimary = true,
                        IsActive = true,
                        IsSoftDeleted = false,
                        AddedById = asset.AddedById,
                        AddedDateTime = DateTime.UtcNow
                    });
                }

                // 🔥 Single insert (Parent + Child)
                await _context.Assets.AddAsync(asset);
                await _context.SaveChangesAsync();

                // 🔥 Get inserted image (from navigation)
                var assetImage = asset.AssetImages.FirstOrDefault();

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
                // ✅ Null-safe fallback
                if (asset == null || asset.Prop == null)
                {
                    _logger.LogWarning("⚠️ GetAssetsByFilterAsync called with null filter");
                    return new List<GetAssetResponseDTO>();
                }

                IQueryable<Asset> query = _context.Assets
                    .AsNoTracking()
                    .Where(a => a.TenantId == asset.Prop.TenantId &&
                                (a.IsSoftDeleted == false || a.IsSoftDeleted == null));

                // 🔹 Filters
                if (asset.AssetId > 0)
                    query = query.Where(a => a.Id == asset.AssetId);

                if (asset.AssetTypeId > 0)
                    query = query.Where(a => a.AssetTypeId == asset.AssetTypeId);

                if (!string.IsNullOrWhiteSpace(asset.SerialNumber))
                    query = query.Where(a => a.SerialNumber != null &&
                                             a.SerialNumber.Contains(asset.SerialNumber));

                if (!string.IsNullOrWhiteSpace(asset.ModelNumber))
                    query = query.Where(a => a.ModelNo != null &&
                                             a.ModelNo.Contains(asset.ModelNumber));

                if (asset.IsActive.HasValue)
                    query = query.Where(a => a.IsActive == asset.IsActive);

                if (asset.IsAssigned.HasValue)
                    query = query.Where(a => a.IsAssigned == asset.IsAssigned);

                // 🔥 FINAL QUERY (Single DB Call)
                var result = await query
                    .OrderByDescending(a => a.Id)
                    .Select(a => new GetAssetResponseDTO
                    {
                        AssetId = a.Id,
                        AssetName = a.AssetName,

                        AssetTypeId = a.AssetTypeId,
                        TypeName = a.AssetType != null ? a.AssetType.TypeName : null,

                        // 🔥 NEW FIELD
                        CategoryId = a.AssetType != null ? a.AssetType.AssetCategoryId : null,
                        CategoryName = a.AssetType != null && a.AssetType.AssetCategory != null ? a.AssetType.AssetCategory.CategoryName : null,
                        HasMultipleUser = a.AssetType != null && a.AssetType.AssetCategory != null ? a.AssetType.AssetCategory.HasMultipleUser : false,

                        SerialNumber = a.SerialNumber,
                        ModelNumber = a.ModelNo,
                        AssetStatusId = a.AssetStatusId,
                        StatusName = a.AssetStatus != null ? a.AssetStatus.StatusName : null,
                        ColorKey = a.AssetStatus != null ? a.AssetStatus.ColorKey : null,

                        Size = a.Size,
                        Weight = a.Weight,
                        Company = a.Company,
                        Color = a.Color,
                        Price = a.Price,

                        IsActive = a.IsActive,
                        IsAssigned = a.IsAssigned,

                        PurchaseDate = a.PurchaseDate,
                        WarrantyExpiryDate = a.WarrantyExpiryDate,

                        // 🔥 Primary Image
                        AssetImagePath = a.AssetImages
                            .Where(i => i.IsPrimary == true && (i.IsSoftDeleted != true) && (i.IsActive == true))
                            .Select(i => i.AssetImagePath)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return result ?? new List<GetAssetResponseDTO>();
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



