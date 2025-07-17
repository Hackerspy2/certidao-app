using Domain;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace Web.ViewComponents
{
    public class PerfilViewComponent : ViewComponent
    {
        private readonly IGenericDataRepository<Pessoa> _pessoa;

        public PerfilViewComponent(IGenericDataRepository<Pessoa> dataRepository)
        {
            _pessoa = dataRepository;
        }

        [ResponseCache(Duration = 300)]
        public Task<IViewComponentResult> InvokeAsync()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(f => f.Type == "IdUsuario");
            var model = _pessoa.Find(a => id != null && a.Id == int.Parse(id.Value));
            return Task.FromResult<IViewComponentResult>(View("_perfil", model));
        }
    }
}