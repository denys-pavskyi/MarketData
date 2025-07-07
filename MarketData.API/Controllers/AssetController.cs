using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.Requests;
using MarketData.BLL.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MarketData.API.Controllers
{
    [Route("api/assets")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAssetService _assetService;


        public AssetController(IAuthService authService, IAssetService assetService)
        {
            _authService = authService;
            _assetService = assetService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAssets()
        {
            // Db contain assets
            var assets = await _assetService.GetAssets();

            if (assets.Count > 0)
            {
                return Ok(assets);
            }

            // Assets was not found in db, syncing them from API
            var tokenResult = await _authService.GetAccessTokenAsync();

            if (!tokenResult.IsSuccess)
            {
                return tokenResult.Error!.ToActionResult();
            }

            var accessToken = tokenResult.Value!;

            var assetsResult = await _assetService.SyncAssetsAsync(accessToken);

            return assetsResult.Match(
                Ok,
                error => error.ToActionResult());

        }

        [HttpPost("prices")]
        public async Task<ActionResult<List<PriceResponseDto>>> GetPrices([FromBody] List<PriceRequestDto> requests)
        {
            var result = await _assetService.GetPricesAsync(requests);
            
            
            return result.Match(
                Ok,
                error => error.ToActionResult());
        }

        //[HttpPost("sync")]
        //public async Task<IActionResult> SyncAssets()
        //{
        //    var tokenResult = await _authService.GetAccessTokenAsync();

        //    if (!tokenResult.IsSuccess)
        //    {
        //        return tokenResult.Error!.ToActionResult();
        //    }

        //    var accessToken = tokenResult.Value!;

        //    var result = await _assetService.SyncAssetsAsync(accessToken);

        //    return result.Match(
        //        _ => NoContent(),
        //        error => error.ToActionResult());

        //}
    }
}
