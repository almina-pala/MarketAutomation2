using MarketAutomation2.API.DTOs.Sales;
using MarketAutomation2.API.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace MarketAutomation2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        // GET: api/Sale
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return Ok(sales);
        }

        // GET: api/Sale/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);

            if (sale == null)
                return NotFound(new
                {
                    Message = "Satış bulunamadı."
                });

            return Ok(sale);
        }

        // POST: api/Sale
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSaleDto dto)
        {
            try
            {
                var sale = await _saleService.CreateSaleAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = sale.SaleId },
                    sale);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}