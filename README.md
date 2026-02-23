# 🚀 OmniQuote Core: Microservices Architecture (C# & Java)

Este repositório contém o ecossistema de alta performance do OmniQuote, uma solução SaaS B2B desenvolvida para processamento financeiro escalável. O projeto utiliza uma arquitetura de microsserviços desacoplada, integrando o poder do ecossistema .NET com a robustez da JVM.

## 💻 Perfil Técnico do Desenvolvedor
O projeto foi concebido e arquitetado por Lucas Duran, desenvolvedor Full-Stack e Estrategista Digital, aplicando as seguintes competências core:

*   **Backend:** Domínio em ecossistema Java (Spring/Standard) e C# (.NET Core).
*   **Sistemas Operacionais:** Desenvolvimento nativo em ambiente Linux, utilizando Terminal para automação e deploy.
*   **Integração:** Especialista em comunicação entre microsserviços e arquitetura orientada a resultados reais.

## 🗄️ Persistência e Estrutura de Dados
Para garantir a integridade dos cálculos financeiros e o histórico de orçamentos, o sistema utiliza uma camada de persistência baseada no padrão Repository:

*   **Arquitetura de Dados:** Estrutura normalizada para armazenamento de Propostas, Taxas e Logs de Auditoria.
*   **Repositório Genérico:** Implementação em C# que isola o banco de dados da lógica de negócio, permitindo migração entre SQL (PostgreSQL/SQL Server) e NoSQL sem impacto no Core.
*   **Data Access Object (DAO):** Camada Java otimizada para escrita de alta performance de snapshots financeiros.

## 🏗️ Estrutura do Projeto
O ecossistema é dividido em dois núcleos principais:

*   **Core-Gateway-CS (C# / .NET 8):** Atua como o API Gateway e Orquestrador. Responsável pela segurança (JWT), resiliência (Health Checks) e interface de entrada de dados.

*   **Financial-Engine-JV (Java 17+):** O "Coração" de cálculos. Um motor especializado em matemática financeira de alta precisão, utilizando processamento paralelo e Streams API.

## 🛠️ Tecnologias e Padrões Implementados (Os 8 Pilares)
Abaixo, os diferenciais técnicos que garantem a viabilidade do sistema:

1.  **Integração Cross-Platform:** Comunicação via HTTP/JSON entre serviços Linux.
2.  **Regras de Negócio Dinâmicas:** Implementação de lógica B2B para taxas de planos.
3.  **Processamento em Lote (Batch):** Uso de Java Streams e C# LINQ para processar massas de dados simultâneas.
4.  **Assincronismo (Non-blocking):** Implementação de `async/await` e `CompletableFuture` para evitar gargalos de I/O.
5.  **Strategy Pattern:** Arquitetura plugável para troca de regras de cálculo sem alteração no código fonte (SOLID).
6.  **Observabilidade:** Sistema de logs estruturados e monitoramento de saúde (Health Checks).
7.  **Segurança (JWT):** Autenticação baseada em tokens com controle de permissões por plano (VIP/Starter).
8.  **Repository Pattern:** Abstração da camada de dados para garantir persistência desacoplada.

## 🚀 Como Executar (Ambiente Linux)
### Pré-requisitos
*   SDK .NET 8+
*   JDK 17+
*   Compilador `javac` e `dotnet-cli`

### Passo 1: Iniciar o Motor Java
```bash
cd Financial-Engine-JV
javac *.java
java MotorFinanceiro
```

### Passo 2: Iniciar o Gateway C#
```bash
cd Core-Gateway-CS
dotnet run
```

## 📊 Diferenciais para o Coordenador
*   **Baixo Acoplamento:** Os serviços são independentes. Se o Java cair, o C# detecta e entra em modo de contingência.
*   **Escalabilidade:** O motor Java foi desenhado para ser "Thread-Safe", permitindo milhares de cálculos por segundo.
*   **Segurança B2B:** Implementação realística de proteção de endpoints.

---
**Desenvolvido por:** Lucas Duran  
**Foco:** Estrategista Digital & Desenvolvedor Full-Stack  
**Status:** Pronto para Produção (MVP Avançado)