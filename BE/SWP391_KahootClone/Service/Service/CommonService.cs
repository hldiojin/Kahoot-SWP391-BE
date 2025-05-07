using Microsoft.AspNetCore.Http;
using Repository.Models;
using Service.IService;

namespace Service.Service
{
    public class CommonService : ICommonService
    {
        private readonly IHttpContextAccessor _httpContext;
        public CommonService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext ;
        }

        public string GetRequestUser()
        {
            try
            {
                var claims = this._httpContext.HttpContext?.User?.Claims;

                if (claims!.Count() > 0)
                {
                    var nameId = claims!.FirstOrDefault(x => x.Type.ToLower().Equals("sub"))?.Value;
                    return nameId;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
