using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            // TODO: Buscar o Id no banco utilizando o EF
            var tarefa = _context.Tarefas.Find(id);
            // TODO: Validar o tipo de retorno. Se não encontrar a tarefa, retornar NotFound,
            // caso contrário retornar OK com a tarefa encontrada

            if(id == null){
                return NotFound();
            }
            return Ok(tarefa);
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            var tarefas = _context.Tarefas.ToList();
            return Ok(tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            // TODO: Buscar  as tarefas no banco utilizando o EF, que contenha o titulo recebido por parâmetro
            // Dica: Usar como exemplo o endpoint ObterPorData

            var tarefa = _context.Tarefas.Where(t => t.Titulo == titulo);

            if(tarefa == null){
                return NotFound("Nenhuma tarefa encontrada com o título especificado.");
            }
            return Ok(tarefa);
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(DateTime data)
        {
            var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date);
            if (tarefa == null || !tarefa.Any())
            {
                return NotFound(new { Error = $"Nenhuma tarefa encontrada para o dia {data:yyyy-MM-dd}." });
            }
            return Ok(tarefa);
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            // TODO: Buscar  as tarefas no banco utilizando o EF, que contenha o status recebido por parâmetro
            // Dica: Usar como exemplo o endpoint ObterPorData
            var tarefas = _context.Tarefas.Where(x => x.Status == status).ToList();

            if (tarefas == null || !tarefas.Any())
            {
                return NotFound("Nenhuma tarefa encontrada com o status especificado.");
            }
            return Ok(tarefas);
        }

        [HttpPost("AdicionarNovaTarefa")]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // TODO: Adicionar a tarefa recebida no EF e salvar as mudanças (save changes)
            if(ModelState.IsValid){
                _context.Add(tarefa);
                _context.SaveChanges();
                return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
            }
            return BadRequest(new { Erro = "Ocorreu algum erro com a criação da sua tarefa, cheque todos os requisitos" });
            
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound("Tarefa não encontrada");

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            // Atualizar as informações da variável tarefaBanco com a tarefa recebida via parâmetro

            if (!string.IsNullOrEmpty(tarefa.Titulo))
            {
                tarefaBanco.Titulo = tarefa.Titulo;
                Console.WriteLine($"Título atualizado para: {tarefa.Titulo}");
            }

            if (!string.IsNullOrEmpty(tarefa.Descricao))
            {
                tarefaBanco.Descricao = tarefa.Descricao;
                Console.WriteLine($"Descrição atualizada para: {tarefa.Descricao}");
            }

            if (tarefa.Data != DateTime.MinValue)
            {
                tarefaBanco.Data = tarefa.Data;
                Console.WriteLine($"Data atualizada para: {tarefa.Data}");
            }else if(tarefaBanco.Status == EnumStatusTarefa.Cancelada){
                tarefaBanco.Status = EnumStatusTarefa.Cancelada;
            }

            if (tarefa.Status != EnumStatusTarefa.Cancelada) // Supondo que EnumStatusTarefa.Default é o valor padrão
            {
                tarefaBanco.Status = tarefa.Status;
                Console.WriteLine($"Status atualizado para: {tarefa.Status}");
            }else{
                tarefaBanco.Status = EnumStatusTarefa.Cancelada;
            }

            // Atualizar a variável tarefaBanco no EF e salvar as mudanças (save changes)

            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();
            
            return Ok(tarefaBanco);
        }


        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            // TODO: Remover a tarefa encontrada através do EF e salvar as mudanças (save changes)
            _context.Tarefas.Remove(tarefaBanco);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
