﻿using Financeiro.CashFlow.Server;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Financial.CashFlow.Sdk
{
    public class LauchClient : ILauchClient
    {
        #region Dependencies

        private readonly LancamentoService.LancamentoServiceClient _lancamentoClient;
        private readonly ILogger<LauchClient> _logger;

        public LauchClient(LancamentoService.LancamentoServiceClient lancamentoClient, ILogger<LauchClient> logger)
        {
            _lancamentoClient = lancamentoClient;
            _logger = logger;
        }

        #endregion END Dependencies

        public async Task<LancamentoResponse> AtualizarLancamentoAsync(LancamentoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lancamentoClient.AtualizarLancamentoAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao atualizar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao atualizar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar lançamento: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<DeletarLancamentoResponse> DeletarLancamentoAsync(LancamentoIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Tenta deletar via gRPC
                var response = await _lancamentoClient.DeletarLancamentoAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao deletar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao deletar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao deletar lançamento: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<LancamentoResponse> RegistrarLancamentoAsync(LancamentoRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _lancamentoClient.RegistrarLancamentoAsync(request, null, null, cancellationToken);
                return response;
            }
            catch (RpcException rpcEx)
            {
                _logger.LogError(rpcEx, "Erro ao registrar lançamento via gRPC: {Message}", rpcEx.Message);
                throw new ApplicationException("Erro ao registrar lançamento", rpcEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao registrar lançamento: {Message}", ex.Message);
                throw;
            }
        }
    }

}
