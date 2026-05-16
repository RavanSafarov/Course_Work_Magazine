using Course_Work_Magazine.BLL.Services.Interfaces;
using Course_Work_Magazine.Common;
using Course_Work_Magazine.DTO.Product_DTOs;
using Course_Work_Magazine.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Course_Work_Magazine.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    // private readonly IStorageService _fileStorage;
    public ProductController(IProductService productService) //IStorageService fileStorage
    {
        _productService = productService;
       // _fileStorage = fileStorage;
    }
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductReadDto>>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<ProductReadDto>>.SuccessResponse(products, "Prodcuts retrieved successfully"));
    }
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductReadDto>>>> GetPaged([FromQuery] ProductQueryParams productQueryParams)
    {
        var product = await _productService.GetPagedAsync(productQueryParams);
        return Ok(ApiResponse<PagedResult<ProductReadDto>>.SuccessResponse(product, "Prodcuts retrieved successfully"));
    }
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<ProductReadDto>>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound(ApiResponse<ProductReadDto>.ErrorResponse($"Product with ID {id} was not found"));
        }
        return Ok(ApiResponse<ProductReadDto>.SuccessResponse(product, "Product retrieved successfully"));
    }
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ProductReadDto>>> Create([FromBody] ProductCreateUpdateDto productCreateUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<ProductReadDto>.ErrorResponse("Invalid request data"));
        }
        var createdProduct = await _productService.CreateAsync(productCreateUpdateDto);
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProduct.Id },
            ApiResponse<ProductReadDto>.SuccessResponse(createdProduct, "Product created successfully")
            );
    }
    [HttpPost("upload-image")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadProductImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        if (!image.ContentType.StartsWith("image/"))
            return BadRequest(new { message = "Only images allowed" });

        if (image.Length > 5 * 1024 * 1024)
            return BadRequest(new { message = "Max file size is 5MB" });

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "products");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{DateTime.Now.Ticks}-{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";

        return Ok(new { success = true, imageUrl });
    }
    //public async Task<IActionResult> UploadProductImage(IFormFile image)
    //{
    //    if (image == null || image.Length == 0)
    //    {
    //        return BadRequest(new { message = "No file uploaded" });
    //    }    


    //    if (!image.ContentType.StartsWith("image/"))
    //    {
    //        return BadRequest(new { message = "Only images allowed" });
    //    }

    //    if (image.Length > 5 * 1024 * 1024)
    //    {
    //        return BadRequest(new { message = "Max file size is 5MB" });
    //    }

    //    var url = await _fileStorage.UploadFileAsync(image);

    //    if (string.IsNullOrEmpty(url))
    //    {
    //        return BadRequest(new { message = "Upload failed" });
    //    }

    //    return Ok(new { success = true, imageUrl = url });
    //}
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<ApiResponse<ProductReadDto>>> Update(int id, [FromBody] ProductCreateUpdateDto productCreateUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<ProductReadDto>.ErrorResponse("Invalid request data"));
        }
        var updatedProduct = await _productService.UpdateAsync(id, productCreateUpdateDto);
        if (updatedProduct is null)
        {
            return NotFound(ApiResponse<ProductReadDto>.ErrorResponse($"Product with ID {id} not found"));
        }
        return Ok(ApiResponse<ProductReadDto>.SuccessResponse(updatedProduct, "Product updated successfully"));
    }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductReadDto>>> Delete(int id)
    {
        var isDeleted = await _productService.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound(ApiResponse<ProductReadDto>.ErrorResponse($"Product with ID {id} not found"));
        }
        return Ok(ApiResponse<ProductReadDto>.SuccessResponse(null!, "Product deleted successfully"));
    }
}
