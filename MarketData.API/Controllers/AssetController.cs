using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.Responses;
using Microsoft.AspNetCore.Http;
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


        [HttpGet]
        public async Task<IActionResult> GetAssets()
        {
            var tokenResult = await _authService.GetAccessTokenAsync();

            if (!tokenResult.IsSuccess)
            {
                return tokenResult.Error!.ToActionResult();
            }

            var accessToken = tokenResult.Value!;

            // Далі логіка роботи з сервісом отримання активів
            // var assets = await _assetService.GetAssetsAsync(accessToken);
            // return Ok(assets);

            return Ok();



        }


    }
}
