using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.DataModels.Data;
using Financeiro.CashFlow.Server;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Financial.CashFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LancamentoController : ControllerBase
    {

        private readonly LancamentoAppDbContext _context;
        private readonly ISender _sender;

        public LancamentoController(LancamentoAppDbContext context, ISender sender)
        {
            _context = context;
            _sender = sender;
        }
                
        [HttpPost("create")]
        public async Task<ActionResult<LancamentoResponse>> CreateLancamento(LancamentoCommand command)
        {
            if (command == null)
            {
                return BadRequest("O comando de lançamento não pode ser nulo.");
            }

            try
            {
                // Envia o comando para o MediatR handler
                var lancamentoResponse = await _sender.Send(command);

                if (!lancamentoResponse.Sucesso)
                {
                    return BadRequest(lancamentoResponse.Mensagem);
                }

                return Ok(lancamentoResponse);
            }
            catch (Exception ex)
            {
                // Tratamento de exceção com retorno de erro 500
                return StatusCode(500, $"Erro ao criar o lançamento: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletarLancamentoResponse>> DeleteLancamento(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Id do lançamento não pode ser vazio.");
            }

            // Criar o comando para deletar o lançamento
            var command = new DeletarLancamentoCommand(id);

            try
            {
                // Enviar o comando para o handler via MediatR
                var result = await _sender.Send(command);

                if (!result.Sucesso)
                {
                    return NotFound(result.Mensagem);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Caso uma exceção seja lançada, retornamos erro 500
                return StatusCode(500, $"Erro ao deletar o lançamento: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<LancamentoResponse>> AtualizarLancamento(Guid id, AtualizarLancamentoCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("O ID na URL não corresponde ao ID do comando.");
            }

            // Enviar o comando para o handler via MediatR
            var result = await _sender.Send(command);

            if (!result.Sucesso)
            {
                return NotFound(result.Mensagem);
            }

            return Ok(result);
        }
    }

}
