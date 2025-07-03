using MarketData.BLL.Interfaces;
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


        //[HttpGet]
        //public async Task<IActionResult> GetAssets()
        //{

        //   var assets = _assetService.



        //}


    }
}
