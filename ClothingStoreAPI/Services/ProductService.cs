using ClothingStoreAPI.DTOs;
using ClothingStoreAPI.Models;
using ClothingStoreAPI.Repositories;

namespace ClothingStoreAPI.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> CreateProductAsync(CreateProductDto createDto)
    {
        var product = new Product
        {
            ProductName = createDto.ProductName,
            Description = createDto.Description,
            CategoryId = createDto.CategoryId,
            BasePrice = createDto.BasePrice,
            Brand = createDto.Brand,
            Material = createDto.Material,
            CareInstructions = createDto.CareInstructions
        };

        var createdProduct = await _productRepository.AddAsync(product);
        var productWithDetails = await _productRepository.GetByIdAsync(createdProduct.ProductId);
        
        return productWithDetails != null ? MapToDto(productWithDetails) : null;
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(updateDto.ProductName))
            product.ProductName = updateDto.ProductName;
        
        if (updateDto.Description != null)
            product.Description = updateDto.Description;
        
        if (updateDto.CategoryId.HasValue)
            product.CategoryId = updateDto.CategoryId.Value;
        
        if (updateDto.BasePrice.HasValue)
            product.BasePrice = updateDto.BasePrice.Value;
        
        if (updateDto.Brand != null)
            product.Brand = updateDto.Brand;
        
        if (updateDto.Material != null)
            product.Material = updateDto.Material;
        
        if (updateDto.CareInstructions != null)
            product.CareInstructions = updateDto.CareInstructions;
        
        if (updateDto.IsActive.HasValue)
            product.IsActive = updateDto.IsActive.Value;

        product.UpdatedAt = DateTime.Now;

        await _productRepository.UpdateAsync(product);
        var updatedProduct = await _productRepository.GetByIdAsync(id);
        
        return updatedProduct != null ? MapToDto(updatedProduct) : null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        if (!await _productRepository.ExistsAsync(id))
        {
            return false;
        }

        await _productRepository.DeleteAsync(id);
        return true;
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.CategoryName,
            BasePrice = product.BasePrice,
            Brand = product.Brand,
            Material = product.Material,
            CareInstructions = product.CareInstructions,
            IsActive = product.IsActive,
            Images = product.ProductImages?.Select(img => new ProductImageDto
            {
                ImageId = img.ImageId,
                ImageUrl = img.ImageUrl,
                IsPrimary = img.IsPrimary,
                DisplayOrder = img.DisplayOrder
            }).ToList(),
            InventoryItems = product.InventoryItems?.Select(inv => new InventoryDto
            {
                InventoryId = inv.InventoryId,
                ProductId = inv.ProductId,
                Size = inv.Size,
                Color = inv.Color,
                QuantityInStock = inv.QuantityInStock,
                ReorderLevel = inv.ReorderLevel,
                LastRestocked = inv.LastRestocked
            }).ToList()
        };
    }
}

