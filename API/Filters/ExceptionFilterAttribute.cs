using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    /// <summary>
    /// Custom exception filter for filtering api exceptions in production mode and returning a proper json response
    /// </summary>
    public sealed class ApiV1ExceptionFilterAttribute : TypeFilterAttribute
    {
        public ApiV1ExceptionFilterAttribute() : base(typeof(ExceptionFilterAttributeImpl))
        {
        }

        /// <summary>
        /// Additional helper class.
        /// We need this to get access to DI.
        /// </summary>
        private sealed class ExceptionFilterAttributeImpl : ExceptionFilterAttribute
        {
            private readonly IHostingEnvironment env;

            public ExceptionFilterAttributeImpl(IHostingEnvironment env)
            {
                this.env = env;
            }

            public override void OnException(ExceptionContext context)
            {
                if (env.IsDevelopment())
                {
                    base.OnException(context);
                }
                else
                {
                    var error = new ErrorResponseModel("An error occurred, please try again later.");
                    context.HttpContext.Response.StatusCode = 500;
                    context.Result = new JsonResult(error);

                    base.OnException(context);
                }

            }

            public override async Task OnExceptionAsync(ExceptionContext context)
            {
                if (env.IsDevelopment())
                {
                    await base.OnExceptionAsync(context);
                }
                else
                {
                    var error = new ErrorResponseModel("An error occurred, please try again later.");
                    context.HttpContext.Response.StatusCode = 500;
                    context.Result = new JsonResult(error);

                    await base.OnExceptionAsync(context);
                }
            }
        }
    }
}
