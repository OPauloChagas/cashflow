using Financeiro.CashFlow.Business.Commands;
using Financeiro.CashFlow.Server;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Financial.CashFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LancamentoController : ControllerBase
    {

        private readonly ISender _sender;

        public LancamentoController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("create")]
        public async Task<ActionResult<LancamentoResponse>> CriarLancamento(LancamentoCommand command)
        {
            if (command == null)
            {
                return BadRequest("O comando de lançamento não pode ser nulo.");
            }

            try
            {
                var lancamentoResponse = await _sender.Send(command);

                if (!lancamentoResponse.Sucesso)
                {
                    return BadRequest(lancamentoResponse.Mensagem);
                }

                return Ok(lancamentoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar o lançamento: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DeletarLancamentoResponse>> DeletarLancamento(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Id do lançamento não pode ser vazio.");
            }

            var command = new DeletarLancamentoCommand(id);

            try
            {
                var result = await _sender.Send(command);

                if (!result.Sucesso)
                {
                    return NotFound(result.Mensagem);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar o lançamento: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> AtualizarLancamento(Guid id, AtualizarLancamentoCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("ID na URL não corresponde ao ID no corpo da requisição.");
            }

            try
            {
                var result = await _sender.Send(command);

                if (result == null)
                {
                    return NotFound("Lançamento não encontrado.");
                }

                if (result.Sucesso)
                {
                    return Ok(result.Mensagem);
                }

                return BadRequest("Erro ao atualizar o lançamento.");
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, $"Erro ao atualizar o lançamento: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }


    }

}
