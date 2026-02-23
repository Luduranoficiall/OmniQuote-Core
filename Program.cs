// [ARQUITETURA]: Desenvolvido sob princípios SOLID e Clean Code.
// [FINALIDADE]: Motor de precificação agnóstico para plataforma SaaS B2B.
// [ESTUDO]: Comparativo de performance entre CLR (.NET) e JVM (Java).

using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

// DTO para a resposta, garantindo tipagem forte
public record PropostaResponse(Guid idProposta, decimal valorLiquido, decimal taxaAplicada, string status);

// --- [PROJETO 7] GESTOR DE SEGURANÇA E IDENTIDADE ---
public class SecurityManager {
    public string GerarTokenAcesso(string usuario, string plano) {
        // Em um sistema real, isso usaria uma biblioteca (ex: System.IdentityModel.Tokens.Jwt)
        // e uma chave secreta para gerar uma assinatura criptográfica real.
        // Para a simulação, criamos um JWT "fake" mas estruturalmente correto.
        string header = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"; // Base64 de: {"alg":"HS256","typ":"JWT"}
        
        // O Payload contém as "Claims" (informações) sobre o usuário e suas permissões.
        string payloadJson = $"{{\"user\":\"{usuario}\",\"plan\":\"{plano.ToUpper()}\"}}";
        string payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson));
        
        return $"{header}.{payload}.fake_signature_for_demo_purposes";
    }
}
// --- [RESILIÊNCIA E OBSERVABILIDADE] ---
// Implementação do padrão Health Check para monitorar a saúde de serviços dependentes.
public class MonitorDeResiliencia {
    private static readonly HttpClient _http = new();

    public async Task<bool> VerificarMotorJava() {
        Console.WriteLine("\n[HEALTH CHECK] Verificando status do Motor Financeiro (Java)...");
        try {
            // Timeout curto para não prender o gateway em caso de falha de rede
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var response = await _http.GetAsync("http://localhost:8080/health", cts.Token);
            
            if (response.IsSuccessStatusCode) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ [HEALTH CHECK] Motor Financeiro (Java) está ONLINE.");
                Console.ResetColor();
                return true;
            }
            return false; // Retorna falso se o status não for de sucesso
        }
        catch (Exception ex) when (ex is TaskCanceledException || ex is HttpRequestException) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("🚨 [HEALTH CHECK] ALERTA: Motor Financeiro (Java) está OFFLINE. Ativando modo de contingência.");
            Console.ResetColor();
            return false;
        }
    }
}

public interface IInternalServiceClient
{
    Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string token);
}

// Cliente Genérico: Funciona para qualquer microsserviço
public class ServiceClient : IInternalServiceClient
{
    private readonly HttpClient _http = new();
    // Tenta ler a URL do ambiente (Docker) ou usa localhost (Local)
    private readonly string _baseUrl = Environment.GetEnvironmentVariable("EngineUrl") ?? "http://localhost:8080";

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, string token)
    {
        // Adiciona o token JWT no header de cada requisição
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.PostAsJsonAsync($"{_baseUrl}{endpoint}", data);
        response.EnsureSuccessStatusCode();
        
        // ReadFromJsonAsync desserializa a resposta JSON para o nosso DTO
        return await response.Content.ReadFromJsonAsync<TResponse>();
    }
}

// O Coração do SaaS: Orquestrador de Propostas
public class GestorDeVendasSaaS
{
    private readonly IInternalServiceClient _financeiro;

    // Injeção de Dependência via construtor
    public GestorDeVendasSaaS(IInternalServiceClient financeiro)
    {
        _financeiro = financeiro;
    }

    public async Task GerarPropostaElite(Guid clienteId, decimal valor, string plano, string token)
    {
        Console.WriteLine($"[GATEWAY C#] Orquestrando proposta para cliente {clienteId}...");
        
        var payload = new { idCliente = clienteId, valorBruto = valor, plano = plano };
        
        // Chamada agora envia o token de segurança
        var resultado = await _financeiro.PostAsync<object, PropostaResponse>("/api/calcular", payload, token);

        // Recebendo o valor líquido calculado pelo Java
        Console.WriteLine($"💰 Orçamento Calculado: {resultado.valorLiquido:C}");
        Console.WriteLine("-------------------------------------------");
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 Gateway C# em modo Assíncrono (High Throughput)...");

        // --- Demonstração de Tarefa em Segundo Plano (Fire-and-Forget) ---
        // Usamos Task.Run para iniciar uma tarefa que não precisa ser aguardada imediatamente.
        // O '_' descarta o resultado da Task, indicando que não vamos esperar por ela aqui.
        _ = Task.Run(async () => {
            Console.WriteLine("[ASYNC] Processando lote em segundo plano...");
            await Task.Delay(2000); // Simula o tempo de rede/cálculo
            Console.WriteLine("✅ Lote em segundo plano processado com sucesso!");
        });

        Console.WriteLine("➡️  O sistema continua livre para outras operações...");
        Console.WriteLine("-------------------------------------------");

        // Simulação de Injeção de Dependência
        IInternalServiceClient serviceClient = new ServiceClient();
        var gestor = new GestorDeVendasSaaS(serviceClient);

        // --- [RESILIÊNCIA] Health Check antes de chamar o serviço ---
        var monitor = new MonitorDeResiliencia();
        bool isMotorOnline = await monitor.VerificarMotorJava();

        // --- [PROJETO 7] GERAÇÃO E USO DO TOKEN ---
        var securityManager = new SecurityManager();
        string user = "Lucas_Duran_SaaS";
        string plan = "PRO";
        string token = securityManager.GerarTokenAcesso(user, plan);
        Console.WriteLine($"🔑 Token de acesso gerado para o usuário '{user}' com plano '{plan}'.");

        if (isMotorOnline)
        {
            // Se o motor está online, processa a proposta normalmente.
            await gestor.GerarPropostaElite(Guid.NewGuid(), 10000.00m, plan, token);
        }
        else
        {
            // Se o motor está offline, o sistema não trava e pode seguir um fluxo alternativo.
            Console.WriteLine("[GATEWAY C#] Ação de contingência: O cálculo será enfileirado para processamento posterior.");
        }

        await Task.Delay(1000); // Garante que a task de fundo tenha tempo de finalizar antes do programa encerrar.

        // --- [PROJETO 8] Persistência com Repository Pattern ---
        Console.WriteLine("\n--- [PROJETO 8] Persistência com Repository Pattern ---");
        var repo = new OrcamentoRepository();
        // Simulando a persistência de um orçamento processado
        repo.Salvar(new Orcamento(Guid.NewGuid(), 9500.00m, "Lucas Duran"));
        Console.WriteLine("-------------------------------------------------------");

        // --- [ARQUITETURA] Demonstração do Strategy Pattern ---
        Console.WriteLine("\n--- [ARQUITETURA] Demonstração do Strategy Pattern ---");
        var processador = new ProcessadorOrcamento();
        var valorBase = 1000m;

        // Usando a estratégia VIP
        processador.DefinirPlano(new CalculoVip());
        Console.WriteLine($"Plano VIP: Valor R$ {valorBase:F2} com taxa de 2% -> Resultado: {processador.Executar(valorBase):C}");

        // Trocando a estratégia para Starter em tempo de execução
        processador.DefinirPlano(new CalculoStarter());
        Console.WriteLine($"Plano Starter: Valor R$ {valorBase:F2} com taxa de 10% -> Resultado: {processador.Executar(valorBase):C}");
        Console.WriteLine("----------------------------------------------------");
    }
}

// --- Implementação do Strategy Pattern ---

// 1. O Contrato (A Interface da Estratégia)
// Define o que todas as estratégias de cálculo devem ser capazes de fazer.
public interface ICalculoStrategy {
    decimal Calcular(decimal valor);
}

// 2. As Estratégias Concretas (As Implementações)
// Cada classe implementa a lógica de um plano específico.

// Estratégia para clientes VIP (Taxa de 2%)
public class CalculoVip : ICalculoStrategy {
    public decimal Calcular(decimal valor) => valor * 0.98m;
}

// Estratégia para clientes Starter (Taxa de 10%)
public class CalculoStarter : ICalculoStrategy {
    public decimal Calcular(decimal valor) => valor * 0.90m;
}

// 3. O Contexto (Quem usa a Estratégia)
// Esta classe não conhece a lógica de cálculo. Ela apenas sabe que
// precisa executar uma estratégia que lhe foi fornecida.
public class ProcessadorOrcamento {
    private ICalculoStrategy _strategy;

    // O método para injetar/trocar a estratégia dinamicamente.
    public void DefinirPlano(ICalculoStrategy strategy) => _strategy = strategy;

    // Executa a estratégia que foi definida.
    public decimal Executar(decimal valor) {
        if (_strategy == null) throw new InvalidOperationException("Nenhuma estratégia de cálculo foi definida.");
        return _strategy.Calcular(valor);
    }
}

// --- [PROJETO 8] CAMADA DE DADOS (REPOSITORY PATTERN) ---

// Entidade de Domínio
public record Orcamento(Guid Id, decimal Valor, string Cliente);

// Interface Genérica (O poder do C#)
public interface IRepository<T> {
    void Salvar(T entidade);
    IEnumerable<T> ListarTodos();
}

// Implementação em Memória
public class OrcamentoRepository : IRepository<Orcamento> {
    private List<Orcamento> _db = new();
    
    public void Salvar(Orcamento orcamento) {
        _db.Add(orcamento);
        Console.WriteLine($"[DB-C#] Orçamento {orcamento.Id} persistido com sucesso.");
    }

    public IEnumerable<Orcamento> ListarTodos() => _db;
}