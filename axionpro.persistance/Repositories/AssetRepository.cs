using AutoMapper;
using axionpro.application.Common.Helpers;
using axionpro.application.Constants;
using axionpro.application.DTOS.AssetDTO.asset;
using axionpro.application.DTOS.AssetDTO.category;
using axionpro.application.DTOS.AssetDTO.status;
using axionpro.application.DTOS.AssetDTO.type;
using axionpro.application.DTOS.Employee.Bank;
using axionpro.application.DTOS.Pagination;
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

        #region Asset

        /// <summary>
        ///   Asset  Codes
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        /// 

        public async Task<GetAssetResponseDTO?> AddAsync(Asset asset, string? path)
        {
            try
            {
                // ===============================
                // 1️⃣ SAVE ASSET (PARENT)
                // ===============================
                await _context.Assets.AddAsync(asset);
                await _context.SaveChangesAsync(); // 🔥 Asset.Id generated

                // ===============================
                // 2️⃣ SAVE ASSET IMAGE (CHILD)
                // ===============================
                var assetImage = new AssetImage
                {
                    AssetId = asset.Id,
                    TenantId = asset.TenantId,
                    AssetImageType = ConstantValues.Web,
                    AssetImagePath = string.IsNullOrWhiteSpace(path) ? null : path,
                    IsPrimary = true,
                    IsActive = true,
                    IsSoftDeleted = false,
                    AddedById = asset.AddedById,
                    AddedDateTime = DateTime.UtcNow
                };

                await _context.AssetImages.AddAsync(assetImage);
                await _context.SaveChangesAsync();

                // ===============================
                // 3️⃣ RETURN INSERTED OBJECT (JOINED DTO)
                // ===============================
                var result = await (
                    from a in _context.Assets
                    join at in _context.AssetTypes
                        on a.AssetTypeId equals at.Id into atj
                    from at in atj.DefaultIfEmpty()

                    join ac in _context.AssetCategories
                        on at.AssetCategoryId equals ac.Id into acj
                    from ac in acj.DefaultIfEmpty()

                    join st in _context.AssetStatuses
                        on a.AssetStatusId equals st.Id into stj
                    from st in stj.DefaultIfEmpty()

                    where a.Id == asset.Id
                    select new GetAssetResponseDTO
                    {
                        AssetId = a.Id,
                        AssetName = a.AssetName,
                        AssetTypeId = a.AssetTypeId,
                        TypeName = at != null ? at.TypeName : null,

                        CategoryId = ac != null ? ac.Id : null,
                        CategoryName = ac != null ? ac.CategoryName : null,

                        AssetStatusId = a.AssetStatusId,
                        StatusName = st != null ? st.StatusName : null,
                        ColorKey = st != null ? st.ColorKey : null,

                        Company = a.Company,
                        ModelNo = a.ModelNo,
                        Size = a.Size,
                        Weight = a.Weight,
                        Color = a.Color,
                        Price = a.Price,
                        SerialNumber = a.SerialNumber,
                        Barcode = a.Barcode,
                        Qrcode = a.Qrcode,

                        AssetImageId = assetImage.Id,
                        AssetImagePath = assetImage.AssetImagePath,
                        AssetImageType = assetImage.AssetImageType,

                        PurchaseDate = a.PurchaseDate,
                        WarrantyExpiryDate = a.WarrantyExpiryDate,
                        IsAssigned = a.IsAssigned,
                        IsActive = a.IsActive
                    }
                ).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while inserting Asset | TenantId={TenantId}",
                    asset?.TenantId);

                return null;
            }
        }




        public Task Update(Asset asset)
        {
            throw new NotImplementedException();
        }
        public async Task UpdateAsync(Asset asset)
        {
            _context.Assets.Update(asset);
            await _context.SaveChangesAsync();
        }

 

        private async Task<string> SaveAssetImageAsync(string assetImagePath, string destinationPath)
        {
            try
            {
                byte[] bytes;

                if (assetImagePath.StartsWith("data:image"))
                {
                    var base64Data = assetImagePath.Substring(assetImagePath.IndexOf(',') + 1);
                    bytes = Convert.FromBase64String(base64Data);
                }
                else if (assetImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    using var httpClient = new HttpClient();
                    bytes = await httpClient.GetByteArrayAsync(assetImagePath);
                }
                else if (File.Exists(assetImagePath))
                {
                    bytes = await File.ReadAllBytesAsync(assetImagePath);
                }
                else
                {
                    _logger.LogWarning("Asset image invalid: {Path}", assetImagePath);
                    return _fileStorageService.GetDefaultImagePath();

                }

                return await _fileStorageService.SaveFileAsync(bytes, Path.GetFileName(destinationPath), Path.GetDirectoryName(destinationPath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving asset image");
                return _fileStorageService.GetDefaultImageUrl();
            }
        }

        public async Task<Asset> GetAssetByIdFromTenantAsync(long id)
        {
            try
            {
                var asset = await _context.Assets.FirstOrDefaultAsync(a => a.Id == id);

                if (asset == null)
                {
                    _logger.LogWarning("Asset with ID {AssetId} not found.", id);
                }

                return asset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching asset with ID {AssetId}.", id);
                throw; // Exception को rethrow कर दें ताकि calling code को पता चल सके
            }
        }

        public async Task<bool> IsAssetCategoryDuplicate(AssetCategory asset)
        {
            try
            {
                return await _context.AssetCategories.AnyAsync(a =>
                    (a.CategoryName == asset.CategoryName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for duplicate asset with ID {AssetId}.", asset.Id);
                throw;  // Exception को रिथ्रो कर दें ताकि calling code को पता चल सके
            }
        }

        public async Task<bool> IsAssetDuplicate(Asset asset)
        {
            try
            {
                return await _context.Assets.AnyAsync(a =>
                    (a.SerialNumber == asset.SerialNumber || a.Barcode == asset.Barcode) && a.Id != asset.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for duplicate asset with ID {AssetId}.", asset.Id);
                throw;  // Exception को रिथ्रो कर दें ताकि calling code को पता चल सके
            }
        }
        public async Task<Asset> UpdateAssetAsync(UpdateAssetRequestDTO assetDto)
        {
            if (assetDto == null || assetDto.Id <= 0)
            {
                _logger.LogWarning("UpdateAssetAsync called with invalid asset DTO or Id.");
                return null;
            }

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                // 🔹 1️⃣ Find Existing Asset
                var existingAsset = await context.Assets
                    .FirstOrDefaultAsync(a => a.Id == assetDto.Id && a.IsSoftDeleted != ConstantValues.IsByDefaultTrue);

                if (existingAsset == null)
                {
                    _logger.LogWarning("Asset with ID {AssetId} not found.", assetDto.Id);
                    return null;
                }

                // 🔹 2️⃣ Update fields if provided
                existingAsset.AssetName = !string.IsNullOrWhiteSpace(assetDto.AssetName) ? assetDto.AssetName : existingAsset.AssetName;
                existingAsset.AssetTypeId = assetDto.AssetTypeId ?? existingAsset.AssetTypeId;
                existingAsset.Company = !string.IsNullOrWhiteSpace(assetDto.Company) ? assetDto.Company : existingAsset.Company;
                existingAsset.ModelNo = !string.IsNullOrWhiteSpace(assetDto.ModelNo) ? assetDto.ModelNo : existingAsset.ModelNo;
                existingAsset.Size = !string.IsNullOrWhiteSpace(assetDto.Size) ? assetDto.Size : existingAsset.Size;
                existingAsset.Weight = !string.IsNullOrWhiteSpace(assetDto.Weight) ? assetDto.Weight : existingAsset.Weight;
                existingAsset.Color = !string.IsNullOrWhiteSpace(assetDto.Color) ? assetDto.Color : existingAsset.Color;
                existingAsset.Barcode = !string.IsNullOrWhiteSpace(assetDto.Barcode) ? assetDto.Barcode : existingAsset.Barcode;
                existingAsset.IsRepairable = assetDto.IsRepairable ?? existingAsset.IsRepairable;
                existingAsset.Price = assetDto.Price ?? existingAsset.Price;
                existingAsset.SerialNumber = !string.IsNullOrWhiteSpace(assetDto.SerialNumber) ? assetDto.SerialNumber : existingAsset.SerialNumber;
                existingAsset.PurchaseDate = assetDto.PurchaseDate ?? existingAsset.PurchaseDate;
                existingAsset.WarrantyExpiryDate = assetDto.WarrantyExpiryDate ?? existingAsset.WarrantyExpiryDate;
                existingAsset.AssetStatusId = assetDto.AssetStatusId ?? existingAsset.AssetStatusId;
                existingAsset.IsAssigned = assetDto.IsAssigned ?? existingAsset.IsAssigned;
                existingAsset.IsActive = assetDto.IsActive ?? existingAsset.IsActive;
                existingAsset.UpdatedById = assetDto.Prop.UserEmployeeId;
                existingAsset.UpdatedDateTime = DateTime.UtcNow;

                // 🔹 3️⃣ Generate QR JSON
                var qrData = new
                {
                    AssetId = existingAsset.Id,
                    AssetCode = $"QR-{existingAsset.AssetTypeId}-{existingAsset.Id:000}",
                    AssetName = existingAsset.AssetName,
                    AssetType = await context.AssetTypes
                                .Where(at => at.Id == existingAsset.AssetTypeId)
                                .Select(at => at.TypeName)
                                .FirstOrDefaultAsync() ?? string.Empty,
                    TenantId = existingAsset.TenantId,
                    PurchaseDate = existingAsset.PurchaseDate,
                    WarrantyExpiryDate = existingAsset.WarrantyExpiryDate,
                    QRCodeVersion = "v1.0"
                };
                string qrJson = JsonConvert.SerializeObject(qrData);
                existingAsset.Qrcode = qrJson;

                context.Assets.Update(existingAsset);
                await context.SaveChangesAsync();

                // 🔹 4️⃣ AssetImage update
                var assetImage = await context.AssetImages
                    .FirstOrDefaultAsync(ai => ai.AssetId == existingAsset.Id && ai.AssetImageType == ConstantValues.Web);

                string qrFolder = _fileStorageService.GetTenantFolderPath(existingAsset.TenantId, "qrcodes");
                string qrFileName = $"ASSET-{existingAsset.AssetTypeId}-{existingAsset.Id:000}-QR.png";
              

               
               

                if (assetImage != null)
                {
                    // 🔹 4a) If new image provided, overwrite
                    if (!string.IsNullOrEmpty(assetDto.AssetImagePath))
                    {
                        string assetFileName = $"ASSET-{existingAsset.AssetTypeId}-{existingAsset.Id:000}-Main.png";
                        string assetFilePath = _fileStorageService.GenerateFilePath(existingAsset.TenantId, "assets", assetFileName);
                        string savedAssetPath = string.Empty;

                        try
                        {
                            if (!string.IsNullOrEmpty(assetDto.AssetImagePath))
                                savedAssetPath = await FileHelper.SaveAssetImageAsync(
                                    assetDto.AssetImagePath,
                                    assetFilePath,
                                    _fileStorageService,
                                    _logger
                                );
                            else
                                savedAssetPath = _fileStorageService.GetDefaultImagePath();
                        }
                        catch (Exception exAssetImg)
                        {
                            _logger.LogError(exAssetImg, "Error saving Asset Image for AssetId {AssetId}", assetDto.Id);
                            savedAssetPath = _fileStorageService.GetDefaultImagePath();
                        }


                        assetImage.AssetImagePath = savedAssetPath;



                    }

                  
                    assetImage.UpdatedDateTime = DateTime.UtcNow;
                    assetImage.UpdatedById =assetDto.Prop.UserEmployeeId;
                    context.AssetImages.Update(assetImage);
                }
                else
                {
                    // 🔹 4b) AssetImage record not exists, create new
                    string assetFilePath;
                    string savedAssetPath;

                    if (!string.IsNullOrEmpty(assetDto.AssetImagePath))
                    {
                        string assetFileName = $"ASSET-{existingAsset.AssetTypeId}-{existingAsset.Id:000}-Main.png";
                        assetFilePath = _fileStorageService.GenerateFilePath(existingAsset.TenantId, "assets", assetFileName);
                        savedAssetPath = await SaveAssetImageAsync(assetDto.AssetImagePath, assetFilePath);
                    }
                    else
                    {
                        assetFilePath = null;
                        savedAssetPath = _fileStorageService.GetDefaultImagePath();
                    }

                    assetImage = new AssetImage
                    {
                        AssetId = existingAsset.Id,
                        TenantId = existingAsset.TenantId,
                        AssetImageType = !string.IsNullOrEmpty(assetDto.AssetImagePath) ? ConstantValues.Web : ConstantValues.Web,
                        AssetImagePath = savedAssetPath,
                       
                        IsActive = true,
                        AddedDateTime = DateTime.UtcNow,
                        AddedById = assetDto.Prop.UserEmployeeId
                    };
                    await context.AssetImages.AddAsync(assetImage);
                }

                await context.SaveChangesAsync();

                return existingAsset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UpdateAssetAsync for AssetId {AssetId}", assetDto?.Id);
                throw;
            }
        }


        /// <summary>
        /// Updates asset information, regenerates QR code and updates images.
        /// Returns true if update succeeds, false if fails or not found.
        /// </summary>
        public async Task<bool> UpdateAssetInfoAsync(UpdateAssetRequestDTO assetDto)
        {
            if (assetDto == null || assetDto.Id <= 0)
            {
                _logger.LogWarning("❌ Invalid Asset DTO or missing Id in UpdateAssetInfoAsync.");
                return false;
            }

            await using var context = await _contextFactory.CreateDbContextAsync();

            try  
            {
                _logger.LogInformation("🔹 Updating asset (ID: {AssetId}, TenantId: {TenantId})", assetDto.Id, assetDto.Prop.TenantId);

                // 🔸 1️⃣ Fetch existing asset with images
                var existingAsset = await context.Assets
                    .Include(a => a.AssetImages)
                    .FirstOrDefaultAsync(a => a.Id == assetDto.Id && a.TenantId == assetDto.Prop.TenantId);

                if (existingAsset == null)
                {
                    _logger.LogWarning("⚠️ Asset not found with Id {AssetId} and TenantId {TenantId}", assetDto.Id, assetDto.Prop.TenantId);
                    return false;
                }

                // 🔸 2️⃣ Update only provided fields (null-safe update)
                existingAsset.AssetName = !string.IsNullOrWhiteSpace(assetDto.AssetName) ? assetDto.AssetName : existingAsset.AssetName;
                existingAsset.AssetTypeId = assetDto.AssetTypeId ?? existingAsset.AssetTypeId;
                existingAsset.Company = !string.IsNullOrWhiteSpace(assetDto.Company) ? assetDto.Company : existingAsset.Company;
                existingAsset.ModelNo = !string.IsNullOrWhiteSpace(assetDto.ModelNo) ? assetDto.ModelNo : existingAsset.ModelNo;
                existingAsset.Size = !string.IsNullOrWhiteSpace(assetDto.Size) ? assetDto.Size : existingAsset.Size;
                existingAsset.Weight = !string.IsNullOrWhiteSpace(assetDto.Weight) ? assetDto.Weight : existingAsset.Weight;
                existingAsset.Color = !string.IsNullOrWhiteSpace(assetDto.Color) ? assetDto.Color : existingAsset.Color;
                existingAsset.Barcode = !string.IsNullOrWhiteSpace(assetDto.Barcode) ? assetDto.Barcode : existingAsset.Barcode;
                existingAsset.IsRepairable = assetDto.IsRepairable ?? existingAsset.IsRepairable;
                existingAsset.Price = assetDto.Price ?? existingAsset.Price;
                existingAsset.SerialNumber = !string.IsNullOrWhiteSpace(assetDto.SerialNumber) ? assetDto.SerialNumber : existingAsset.SerialNumber;
                existingAsset.PurchaseDate = assetDto.PurchaseDate ?? existingAsset.PurchaseDate;
                existingAsset.WarrantyExpiryDate = assetDto.WarrantyExpiryDate ?? existingAsset.WarrantyExpiryDate;
                existingAsset.AssetStatusId = assetDto.AssetStatusId ?? existingAsset.AssetStatusId;
                existingAsset.IsAssigned = assetDto.IsAssigned ?? existingAsset.IsAssigned;
                existingAsset.IsActive = assetDto.IsActive ?? existingAsset.IsActive;
                existingAsset.UpdatedById = assetDto.Prop.UserEmployeeId;
                existingAsset.UpdatedDateTime = DateTime.UtcNow;

                // 🔸 3️⃣ Generate new QR data
                var assetTypeName = await context.AssetTypes
                    .Where(at => at.Id == existingAsset.AssetTypeId)
                    .Select(at => at.TypeName)
                    .FirstOrDefaultAsync() ?? string.Empty;
                var qrData = new
                {
                    AssetId = existingAsset.Id,
                    AssetCode = $"QR-{existingAsset.AssetTypeId}-{existingAsset.Id:000}",
                    AssetName = existingAsset.AssetName,
                    AssetType = assetTypeName,
                    TenantId = existingAsset.TenantId,

                    PurchaseDate = existingAsset.PurchaseDate.HasValue
                        ? existingAsset.PurchaseDate.Value.ToString("yyyy-MM-dd"): null,

                    WarrantyExpiryDate = existingAsset.WarrantyExpiryDate,
                    QRCodeVersion = "v1.0"
                };

                string qrJson = JsonConvert.SerializeObject(qrData);
                existingAsset.Qrcode = qrJson;
                // 🔸 5️⃣ Update or add QR code image record
               
                 

                // 🔸 6️⃣ Handle Main Asset Image (if provided)
                if (!string.IsNullOrEmpty(assetDto.AssetImagePath) && File.Exists(assetDto.AssetImagePath))
                {
                    string assetFileName = $"ASSET-{existingAsset.AssetTypeId}-{existingAsset.Id:000}-Main.png";
                    string assetFolder = _fileStorageService.GetTenantFolderPath(existingAsset.TenantId, "assets");
                    string assetFilePath = _fileStorageService.GenerateFilePath(existingAsset.TenantId, "assets", assetFileName);

                    File.Copy(assetDto.AssetImagePath, assetFilePath, true);

                    var mainImage = existingAsset.AssetImages.FirstOrDefault(ai => ai.AssetImageType == ConstantValues.Web);
                    if (mainImage != null)
                    {
                        mainImage.AssetImagePath = assetFilePath;
                        mainImage.UpdatedById = assetDto.Prop.UserEmployeeId;
                        mainImage.UpdatedDateTime = DateTime.UtcNow;
                        context.AssetImages.Update(mainImage);
                    }
                    else
                    {
                        await context.AssetImages.AddAsync(new AssetImage
                        {
                            AssetId = existingAsset.Id,
                            TenantId = existingAsset.TenantId,
                            AssetImageType = ConstantValues.Web,
                            AssetImagePath = assetFilePath,
                            IsActive = true,
                            UpdatedById = assetDto.Prop.UserEmployeeId,
                            UpdatedDateTime = DateTime.UtcNow
                        });
                    }
                }

                // 🔸 7️⃣ Commit all changes
                context.Assets.Update(existingAsset);
                var changes = await context.SaveChangesAsync();

                if (changes > 0)
                {
                    _logger.LogInformation("✅ Asset updated successfully (ID: {AssetId})", existingAsset.Id);
                    return true;
                }

                _logger.LogWarning("⚠️ No changes detected for Asset ID: {AssetId}", existingAsset.Id);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error occurred while updating asset info (ID: {AssetId})", assetDto?.Id);
                return false;
            }
        }


        public async Task<List<GetAssetResponseDTO>> GetInsertedAssetAsync(
      long tenantId,
      bool isActive)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching latest 10 assets for TenantId: {TenantId}",
                    tenantId);

                var result = await _context.Assets
                    .AsNoTracking()
                    .Where(a =>
                        a.TenantId == tenantId &&
                        a.IsActive == isActive &&
                        a.IsSoftDeleted != true)
                    .OrderByDescending(a => a.Id) // 🔥 latest first
                    .Select(a => new GetAssetResponseDTO
                    {
                        // =========================
                        // ASSET
                        // =========================
                        AssetId = a.Id,
                        AssetName = a.AssetName,
                        Company = a.Company,
                        ModelNo = a.ModelNo,
                        Size = a.Size,
                        Weight = a.Weight,
                        Color = a.Color,
                        IsRepairable = a.IsRepairable,
                        Price = a.Price,
                        SerialNumber = a.SerialNumber,
                        Barcode = a.Barcode,
                        Qrcode = a.Qrcode,
                        PurchaseDate = a.PurchaseDate,
                        WarrantyExpiryDate = a.WarrantyExpiryDate,
                        IsAssigned = a.IsAssigned,
                        IsActive = a.IsActive,

                        // =========================
                        // ASSET TYPE
                        // =========================
                        AssetTypeId = a.AssetTypeId,
                        TypeName = a.AssetType != null
                            ? a.AssetType.TypeName
                            : null,

                        // =========================
                        // CATEGORY (via AssetType)
                        // =========================
                        CategoryId = a.AssetType != null
                            ? a.AssetType.AssetCategory.Id
                            : null,

                        CategoryName = a.AssetType != null
                            ? a.AssetType.AssetCategory.CategoryName
                            : null,

                        // =========================
                        // STATUS
                        // =========================
                        AssetStatusId = a.AssetStatusId,
                        StatusName = a.AssetStatus != null
                            ? a.AssetStatus.StatusName
                            : null,

                        ColorKey = a.AssetStatus != null
                            ? a.AssetStatus.ColorKey
                            : null,

                        // =========================
                        // IMAGE (LATEST ACTIVE)
                        // =========================
                        AssetImageId = a.AssetImages
                            .Where(i =>
                                i.IsActive &&
                                i.IsSoftDeleted != true)
                            .OrderByDescending(i => i.Id)
                            .Select(i => (long?)i.Id)
                            .FirstOrDefault(),

                        AssetImagePath = a.AssetImages
                            .Where(i =>
                                i.IsActive &&
                                i.IsSoftDeleted != true)
                            .OrderByDescending(i => i.Id)
                            .Select(i => i.AssetImagePath)
                            .FirstOrDefault(),

                        AssetImageType = a.AssetImages
                            .Where(i =>
                                i.IsActive &&
                                i.IsSoftDeleted != true)
                            .OrderByDescending(i => i.Id)
                            .Select(i => (int?)i.AssetImageType)
                            .FirstOrDefault()
                    })
                    .Take(10)
                    .ToListAsync();

                _logger.LogInformation(
                    "Fetched {Count} assets for TenantId: {TenantId}",
                    result.Count,
                    tenantId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "❌ Error while fetching latest assets for TenantId: {TenantId}",
                    tenantId);

                throw;
            }
        }


        public async Task<List<GetAssetResponseDTO>> GetAllAsync(
               long tenantId,
               bool isActive)
        {
            try
            {
                
                _logger.LogInformation(
                    "Fetching latest 10 assets for TenantId: {TenantId}",
                    tenantId);

                var assets = await (
                    from a in _context.Assets
                    where a.TenantId == tenantId
                          && a.IsActive == isActive
                          && (a.IsSoftDeleted == false || a.IsSoftDeleted == null)
                    orderby a.Id descending   // 🔥 latest first
                    select new GetAssetResponseDTO
                    {
                        AssetId = a.Id,
                        AssetName = a.AssetName,
                        AssetTypeId = a.AssetTypeId,
                        Company = a.Company,
                        Color = a.Color,
                        IsRepairable = a.IsRepairable,
                        Price = a.Price,
                        SerialNumber = a.SerialNumber,
                        Barcode = a.Barcode,
                        PurchaseDate = a.PurchaseDate,
                        WarrantyExpiryDate = a.WarrantyExpiryDate,
                        IsAssigned = a.IsAssigned,
                        Qrcode = a.Qrcode,

                        // ✅ Main Image (latest active)
                        AssetImagePath = _context.AssetImages
                            .Where(img =>
                                img.AssetId == a.Id &&
                                img.IsActive &&
                                img.IsSoftDeleted != true)
                            .Select(img => img.AssetImagePath)
                            .FirstOrDefault()
                    })
                    .Take(10)                  // ✅ sirf 10 records
                    .ToListAsync();

                _logger.LogInformation(
                    "Fetched {Count} assets for TenantId: {TenantId}",
                    assets.Count,
                    tenantId);

                return assets;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while fetching assets for TenantId: {TenantId}",
                    tenantId);

                throw;
            }
        }



        public async Task<bool> DeleteAssetAsync(DeleteAssetReqestDTO? asset)
        {
            try
            {
                if (asset == null || asset.Id <= 0)
                {
                    _logger.LogWarning("DeleteAssetAsync called with invalid asset DTO or Id.");
                    return false;
                }

                // Corrected query: use x for filtering
                var existingAsset = await _context.Assets
                    .FirstOrDefaultAsync(x => x.Id == asset.Id &&
                                              x.IsSoftDeleted != true);

                if (existingAsset == null)
                {
                    _logger.LogWarning("Asset with Id {Id} not found for TenantId {TenantId}.", asset.Id, asset.Prop.TenantId);
                    return false; // Or throw custom NotFoundException
                }
                if (existingAsset.IsAssigned == ConstantValues.IsByDefaultTrue)
                    return false;

                // Update only the tracked entity
                existingAsset.IsSoftDeleted = ConstantValues.IsByDefaultTrue;
                existingAsset.IsActive = ConstantValues.IsByDefaultFalse;
                existingAsset.SoftDeletedById = asset.Prop.UserEmployeeId;
                existingAsset.DeletedDateTime = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while soft deleting Asset.");
                return false;
            }

        }


        /// <summary>
        /// Fetch assets based on filter criteria safely.
        /// Returns all assets matching TenantId, TypeId (optional) with latest AssetImage and QRCode image.
        /// </summary>
        /// <param name="asset">Filter DTO containing TenantId, TypeId, etc.</param>
        /// <returns>List of GetAssetResponseDTO</returns>
        /// 

        /// <summary>
        /// Retrieves filtered assets based on multiple optional parameters.
        /// Mandatory: TenantId, RoleId, EmployeeId.
        /// </summary>
        /// 

        public async Task<List<GetAssetResponseDTO>> GetAssetsByFilterAsync(
    GetAssetRequestDTO asset)
        {
            if (asset == null)
                throw new ArgumentNullException(nameof(asset));

            if (asset.Prop == null || asset.Prop.TenantId <= 0)
                throw new ArgumentException("TenantId is mandatory.");

            int pageNumber = asset.PageNumber <= 0 ? 1 : asset.PageNumber;
            int pageSize = asset.PageSize <= 0 ? 10 : asset.PageSize;
            int skip = (pageNumber - 1) * pageSize;

            await using var context =
                await _contextFactory.CreateDbContextAsync();

            try
            {
                // ===============================
                // 1️⃣ BASE QUERY
                // ===============================
                IQueryable<Asset> query = context.Assets
                    .Where(a =>
                        a.TenantId == asset.Prop.TenantId &&
                        a.IsSoftDeleted != true);

                // ===============================
                // 2️⃣ DYNAMIC FILTERS (NULL SAFE)
                // ===============================
                if (asset.AssetId > 0)
                    query = query.Where(a => a.Id == asset.AssetId);

                if (asset.AssetTypeId > 0)
                    query = query.Where(a => a.AssetTypeId == asset.AssetTypeId);

                if (!string.IsNullOrWhiteSpace(asset.SerialNumber))
                    query = query.Where(a =>
                        a.SerialNumber != null &&
                        a.SerialNumber.Contains(asset.SerialNumber));

                if (!string.IsNullOrWhiteSpace(asset.ModelNumber))
                    query = query.Where(a =>
                        a.ModelNo != null &&
                        a.ModelNo.Contains(asset.ModelNumber));

                if (asset.PurchasedDateTime.HasValue)
                {
                    var date = asset.PurchasedDateTime.Value.Date;
                    query = query.Where(a =>
                        a.PurchaseDate.HasValue &&
                        a.PurchaseDate.Value.Date == date);
                }

                if (asset.AssetStatusId > 0)
                    query = query.Where(a => a.AssetStatusId == asset.AssetStatusId);

                if (asset.IsAssigned.HasValue)
                    query = query.Where(a => a.IsAssigned == asset.IsAssigned);

                if (asset.IsActive.HasValue)
                    query = query.Where(a => a.IsActive == asset.IsActive);

                // ===============================
                // 3️⃣ JOINS + PAGINATION + PROJECTION
                // ===============================
                var result = await (
                    from a in query

                    join at in context.AssetTypes
                        on a.AssetTypeId equals at.Id into atj
                    from at in atj.DefaultIfEmpty()

                    join ac in context.AssetCategories
                        on at.AssetCategoryId equals ac.Id into acj
                    from ac in acj.DefaultIfEmpty()

                    join st in context.AssetStatuses
                        on a.AssetStatusId equals st.Id into stj
                    from st in stj.DefaultIfEmpty()

                    orderby a.Id descending

                    select new GetAssetResponseDTO
                    {
                        AssetId = a.Id,
                        AssetName = a.AssetName,

                        AssetTypeId = a.AssetTypeId,
                        TypeName = at != null ? at.TypeName : null,

                        CategoryId = ac != null ? ac.Id : null,
                        CategoryName = ac != null ? ac.CategoryName : null,

                        Company = a.Company,
                        ModelNo = a.ModelNo,
                        Size = a.Size,
                        Weight = a.Weight,
                        Color = a.Color,
                        IsRepairable = a.IsRepairable,
                        Price = a.Price,
                        SerialNumber = a.SerialNumber,
                        Barcode = a.Barcode,
                        Qrcode = a.Qrcode,

                        PurchaseDate = a.PurchaseDate,
                        WarrantyExpiryDate = a.WarrantyExpiryDate,

                        AssetStatusId = a.AssetStatusId,
                        StatusName = st != null ? st.StatusName : null,
                        ColorKey = st != null ? st.ColorKey : null,

                        IsAssigned = a.IsAssigned,
                        IsActive = a.IsActive,

                        // 🔹 Latest Image (NULL SAFE)
                        AssetImagePath = context.AssetImages
                            .Where(i =>
                                i.AssetId == a.Id &&
                                i.IsSoftDeleted != true &&
                                i.IsActive)
                            .OrderByDescending(i => i.UpdatedDateTime)
                            .Select(i => i.AssetImagePath)
                            .FirstOrDefault(),

                        AssetImageType = context.AssetImages
                            .Where(i =>
                                i.AssetId == a.Id &&
                                i.IsSoftDeleted != true &&
                                i.IsActive)
                            .OrderByDescending(i => i.UpdatedDateTime)
                            .Select(i => i.AssetImageType)
                            .FirstOrDefault()
                    }
                )
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in GetAssetsByFilterAsync");
                throw;
            }
        }








        #endregion



    }
}



