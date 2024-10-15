using Financeiro.CashFlow.DataModels;
using Financeiro.CashFlow.DataModels.Data;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;


namespace Financeiro.CashFlow.Server.Services
{
    public class LauchService : LaunchRegisterService.LaunchRegisterServiceBase
    {
        private readonly LancamentoAppDbContext _context;
        private readonly ILogger<LauchService> _logger;


        public LauchService(LancamentoAppDbContext context, ILogger<LauchService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task<LaunchResponse> RegisterLaunch(LaunchRequest request, ServerCallContext context)
        {
            try
            {

                var lancamento = new LancamentoDataModel
                {
                    Id = Guid.Parse(request.Id),
                    Tipo = request.Type,
                    Valor = request.Value,
                    Descricao = request.Description,
                    Data = request.Date,
                    ClienteId = request.ClientId
                };

                _context.Lancamentos.Add(lancamento);
                await _context.SaveChangesAsync();


                return new LaunchResponse
                {
                    Id = lancamento.Id.ToString(),
                    Success = true,
                    Message = "Lançamento registrado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar lançamento: {Message}", ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao registrar o lançamento: {ex.Message}"));
            }
        }

        public override async Task<LaunchResponse> UpdateLaunch(LaunchRequest request, ServerCallContext context)
        {
            try
            {
                var lancamentoExistente = await _context.Lancamentos
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.Id));

                if (lancamentoExistente == null)
                {
                    return new LaunchResponse
                    {
                        Success = false,
                        Message = "Lançamento não encontrado"
                    };
                }

                var lancamentoAtualizado = lancamentoExistente with
                {
                    Tipo = request.Type,
                    Valor = request.Value,
                    Descricao = request.Description,
                    Data = request.Date,
                    ClienteId = request.ClientId
                };

                _context.Lancamentos.Update(lancamentoAtualizado);
                await _context.SaveChangesAsync();

                return new LaunchResponse
                {
                    Id = lancamentoExistente.Id.ToString(),
                    Success = true,
                    Message = "Lançamento atualizado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar lançamento: {Message}", ex.Message);

                // Retorna uma resposta de erro apropriada
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao atualizar o lançamento: {ex.Message}"));
            }
        }

        public override async Task<DeleteLaunchResponse> DeleteLaunch(LaunchIdRequest request, ServerCallContext context)
        {
            try
            {

                var lancamentoExistente = await _context.Lancamentos.FindAsync(Guid.Parse(request.Id));

                if (lancamentoExistente == null)
                {
                    return new DeleteLaunchResponse
                    {
                        Success = false,
                        Message = "Lançamento não encontrado"
                    };
                }

                _context.Lancamentos.Remove(lancamentoExistente);
                await _context.SaveChangesAsync();

                return new DeleteLaunchResponse
                {
                    Success = true,
                    Message = "Lançamento deletado com sucesso!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar lançamento: {Message}", ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, $"Erro ao deletar o lançamento: {ex.Message}"));
            }
        }
    }
}
