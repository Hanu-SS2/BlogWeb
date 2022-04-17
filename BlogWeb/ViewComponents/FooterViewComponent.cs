using Microsoft.AspNetCore.Mvc;

namespace BlogWeb.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            return await Task.Factory.StartNew(() => { 
                
                return View(); 
            });
        }
    }
}
