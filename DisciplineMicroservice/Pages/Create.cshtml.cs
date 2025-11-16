using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DisciplineMicroservice.DTO;

namespace DisciplineMicroservice.Pages
{
    public class CreateModel : PageModel
    {
        [BindProperty]
        public DisciplineDTO Discipline { get; set; }
        private readonly HttpDiscipline httpDiscipline;

        public CreateModel(IHttpDisciplineFactory f)
        {
            httpDiscipline = f.CreateDiscipline("disciplineApi");
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var res = httpDiscipline.PostAsJsonAsync("/api/Discipline/insert", Discipline);

            return RedirectToPage("Index");


        }
    }
}