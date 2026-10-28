# Caramelo.MvvmApp

**Caramelo.MvvmApp** é um micro-framework MVVM moderno para aplicações desktop em **.NET** (`net8.0`, `net9.0` e `net10.0`), construído sobre o [ReactiveUI](https://www.reactiveui.net/) e integrado nativamente ao ecossistema `Microsoft.Extensions` (Injeção de Dependência, Configuração e Logging), com suporte completo para **Avalonia UI** e **WPF**.

---

## Principais Recursos

- **Inicialização Fluente (`MvvmAppBuilder`)**: Padrão familiar inspirado no `HostApplicationBuilder` do .NET, integrando `IServiceCollection`, `IConfiguration` (`appsettings.json`, `appsettings.Debug.json` / `appsettings.Release.json`) e `ILoggingBuilder` ao `Splat` / `ReactiveUI`.
- **Compartilhamento Total de ViewModels**: Escreva suas ViewModels e regras de navegação uma única vez em um projeto base e reutilize-as tanto no **Avalonia UI** quanto no **WPF**.
- **Navegação Tipada e Assíncrona (`INavigationService`)**:
  - Navegação simples (`GoToAsync<TViewModel>()`).
  - Passagem de parâmetros fortemente tipados (`MvvmViewModel<TParameter>`).
  - Retorno assíncrono de resultados entre telas (`MvvmResultViewModel<TResult>` e `MvvmViewModel<TParameter, TResult>`).
  - Guardas de navegação (`CanNavigateAsync`) para interceptar saídas e evitar perda de alterações não salvas.
- **Navegação em Abas (`TabbedRoutedViewHost`)**:
  - Controle visual nativo para Avalonia e WPF que gerencia múltiplas abas, cada uma com sua própria pilha de navegação independente (`RoutingState`).
  - Controle declarativo por atributos: `[NewTab]`, `[CurrentTab]` e `[CanClose(true/false)]`.
- **Serviço de Diálogos (`IDialogService`)**:
  - Diálogos prontos para uso: Informação (`InfoAsync`), Aviso (`WarnAsync`), Erro (`ErroAsync`), Confirmação (`ConfirmAsync`) e Entrada de Texto (`InputAsync`).
  - Suporte a diálogos customizados baseados em `MvvmDialogViewModel<TParameter, TResult>`.
- **Ciclo de Vida de Views e Splash Screen**:
  - Hooks de ciclo de vida nas ViewModels (`ViewCreated`, `ViewAppearing`, `ViewAppeared`, `ViewDisappearing`, `ViewDisappeared`, `ViewDestroy`).
  - Suporte integrado a tela de abertura via `UseSplash<TSplashView, TSplashViewModel>()`.
- **Auto-registro de Views e ViewModels**:
  - Descoberta e registro automático de pares `IViewFor<TViewModel>` e `TViewModel` por Assembly via `AddViewAndModelFromAssembly()`.

---

## Pacotes e Estrutura do Projeto

| Pacote / Projeto | Descrição | Frameworks Alvo |
| :--- | :--- | :--- |
| **`Caramelo.MvvmApp`** | Núcleo do framework (Builder, ViewModels base, `INavigationService`, `IDialogService`, Atributos e Contratos) | `net8.0`, `net9.0`, `net10.0` |
| **`Caramelo.MvvmApp.Avalonia`** | Implementação de controles (`MvvmWindow`, `MvvmUserControl`, `TabbedRoutedViewHost`) e diálogos para **Avalonia UI** | `net8.0`, `net9.0`, `net10.0` |
| **`Caramelo.MvvmApp.WPF`** | Implementação de controles (`MvvmWindow`, `MvvmUserControl`, `TabbedRoutedViewHost`) e diálogos para **WPF** | `net8.0-windows`, `net9.0-windows`, `net10.0-windows` |
| **`Caramelo.MvvmApp.Demo.Core`** | Biblioteca compartilhada com as ViewModels de demonstração | `net10.0` |
| **`Caramelo.MvvmApp.Avalonia.Demo`** | Aplicação de demonstração em Avalonia UI | `net10.0` |
| **`Caramelo.MvvmApp.WPF.Demo`** | Aplicação de demonstração em WPF | `net10.0-windows` |
| **`Caramelo.MvvmApp.Tests`** | Testes unitários de navegação e coordenação de abas | `net10.0` |

---

## Instalação

Adicione o pacote correspondente à plataforma da sua interface gráfica:

### Para Avalonia UI
```bash
dotnet add package Caramelo.MvvmApp.Avalonia
```

### Para WPF
```bash
dotnet add package Caramelo.MvvmApp.WPF
```

### Em bibliotecas compartilhadas (Core / ViewModels)
```bash
dotnet add package Caramelo.MvvmApp
```

---

## Guia de Início Rápido

### 1. Criando a ViewModel Principal (`AppViewModel`)

A `AppViewModel` gerencia o roteador principal (`Router`) da janela da aplicação e oferece métodos utilitários como `FinishApp()` e `RestartApp()`.

```csharp
using Caramelo.MvvmApp.ViewModel;
using ReactiveUI.SourceGenerators;

public partial class MainAppViewModel : AppViewModel
{
    public MainAppViewModel(IServiceProvider service) : base(service)
    {
    }

    [ReactiveCommand]
    private async Task OpenHomeAsync()
    {
        await Navigation.GoToAsync<HomeViewModel>();
    }
}
```

### 2. Configurando a Janela Principal (`MvvmWindow` + `TabbedRoutedViewHost`)

No XAML/AXAML da sua janela principal, utilize `caramelo:MvvmWindow` e associe o `TabbedRoutedViewHost` à propriedade `Router`:

**Avalonia (`MainAppView.axaml`) / WPF (`MainAppView.xaml`):**
```xml
<caramelo:MvvmWindow x:TypeArguments="vm:MainAppViewModel"
                     xmlns="https://github.com/avaloniaui"
                     xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                     xmlns:vm="clr-namespace:MeuApp.ViewModels"
                     xmlns:caramelo="https://github.com/rftd/Caramelo.MvvmApp"
                     x:Class="MeuApp.Views.MainAppView"
                     Title="Minha Aplicação">

    <DockPanel>
        <!-- Menu ou Barra de Navegação Superior -->
        <Button DockPanel.Dock="Top" Content="Início" Command="{Binding OpenHomeCommand}" />

        <!-- Host de Navegação em Abas -->
        <caramelo:TabbedRoutedViewHost Router="{Binding Router}" />
    </DockPanel>
</caramelo:MvvmWindow>
```

### 3. Inicializando a Aplicação (`Program.cs`)

#### No Avalonia UI
```csharp
public class App : MvvmApplication<MainAppViewModel>
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}

class Program
{
    [STAThread]
    public static void Main()
    {
        var builder = MvvmApp.CreateBuilder();

        builder.UseAvalonia<App, MainAppViewModel, MainAppView>();

        // Registra automaticamente todas as Views e ViewModels do assembly
        builder.Services.AddViewAndModelFromAssembly(typeof(MainAppViewModel).Assembly);

        var app = builder.Build();
        app.Run();
    }
}
```

#### No WPF
```csharp
public partial class App : MvvmApplication<MainAppViewModel>
{
}

public class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var builder = MvvmApp.CreateBuilder();

        builder.UseWpf<App, MainAppViewModel, MainAppView>();

        // Registra automaticamente todas as Views e ViewModels do assembly
        builder.Services.AddViewAndModelFromAssembly(typeof(MainAppViewModel).Assembly);

        var app = builder.Build();
        app.Run();
    }
}
```

---

## Funcionalidades em Detalhes

### Navegação entre Telas, Parâmetros e Resultados

O `INavigationService` está disponível diretamente na propriedade `Navigation` de qualquer `MvvmViewModel`:

1. **ViewModel Padrão (`MvvmViewModel`)**:
   ```csharp
   await Navigation.GoToAsync<HomeViewModel>();
   ```

2. **Recebendo Parâmetros (`MvvmViewModel<TParameter>`)**:
   ```csharp
   public partial class DetailViewModel : MvvmViewModel<string>
   {
       public DetailViewModel(IServiceProvider service) : base(service) { }

       public override void Initialize(string parameter)
       {
           Title = $"Detalhe: {parameter}";
       }
   }

   // Chamada:
   await Navigation.GoToAsync<DetailViewModel, string>("Item #42");
   ```

3. **Recebendo Parâmetros e Retornando Resultado (`MvvmViewModel<TParameter, TResult>`)**:
   ```csharp
   public partial class PromptResultViewModel : MvvmViewModel<string, string>
   {
       public PromptResultViewModel(IServiceProvider service) : base(service) { }

       public override void Initialize(string parameter)
       {
           // Inicializa com o parâmetro recebido
       }

       [ReactiveCommand]
       private async Task ConfirmAsync()
       {
           // Retorna à tela anterior devolvendo o resultado
           await Navigation.GoBackAsync(this, "Resultado Confirmado");
       }
   }

   // Chamada aguardando o resultado:
   string? resultado = await Navigation.GoToAsync<PromptResultViewModel, string, string>("Configuração Inicial");
   ```

---

### Controle de Abas por Atributos

Quando utilizado em conjunto com o `TabbedRoutedViewHost`, você pode decorar suas ViewModels para definir o comportamento de abertura e fechamento de abas:

```csharp
using Caramelo.MvvmApp.Navigation;
using Caramelo.MvvmApp.ViewModel;

// Abre na aba atual (padrão) e impede que o usuário feche esta aba
[CurrentTab]
[CanClose(false)]
public partial class HomeViewModel : MvvmViewModel
{
    public HomeViewModel(IServiceProvider service) : base(service)
    {
        Title = "Início";
    }
}

// Sempre abre em uma nova aba ao navegar via GoToAsync e permite fechá-la
[NewTab(canClose: true)]
public partial class RelatorioViewModel : MvvmViewModel
{
    public RelatorioViewModel(IServiceProvider service) : base(service)
    {
        Title = "Relatório";
    }
}
```

---

### Navegação Protegida (Navigation Guards)

Sobrescreva `CanNavigateAsync` na sua ViewModel para confirmar com o usuário antes de sair da tela ou fechar a aba:

```csharp
public partial class CadastroViewModel : MvvmViewModel
{
    [Reactive] private bool hasUnsavedChanges = true;

    public CadastroViewModel(IServiceProvider service) : base(service)
    {
        Title = "Cadastro";
    }

    public override async Task<bool> CanNavigateAsync(NavigationMode mode, IMvvmViewModel? viewModel)
    {
        if (!HasUnsavedChanges)
            return true;

        return await Dialogs.ConfirmAsync(
            "Alterações Pendentes",
            "Existem alterações não salvas. Deseja realmente sair?");
    }
}
```

---

### Serviço de Diálogos (`IDialogService`)

Todas as ViewModels derivadas de `MvvmViewModel` possuem acesso direto à propriedade `Dialogs`:

```csharp
// Mensagens rápidas
await Dialogs.InfoAsync("Operação concluída com sucesso!");
await Dialogs.WarnAsync("Atenção: verifique os campos informados.");
await Dialogs.ErroAsync("Não foi possível conectar ao servidor.");

// Diálogo de confirmação (Sim / Não)
bool confirmado = await Dialogs.ConfirmAsync("Excluir Registro", "Deseja realmente excluir este item?");

// Diálogo de entrada de dados
string nome = await Dialogs.InputAsync("Identificação", "Digite o seu nome:");
```

Para criar um diálogo customizado, herde de `MvvmDialogViewModel<TParameter, TResult>` (onde `TParameter : DialogOptions`) e chame `SetResult(resultado)` quando desejar encerrar o diálogo e retornar o valor para `Dialogs.ShowAsync<TViewModel, TResult, TParameter>(opcoes)`.

---

### Splash Screen

Para exibir uma tela de carregamento antes da janela principal, crie uma ViewModel herdando de `MvvmSplashViewModel` e registre-a no builder:

```csharp
public class MeuSplashViewModel : MvvmSplashViewModel
{
    public MeuSplashViewModel(IServiceProvider service) : base(service) { }

    public override async void ViewAppeared()
    {
        // Carregue recursos, migrações ou configurações iniciais
        await Task.Delay(2000);
        CloseSplash();
    }
}

// No Program.cs:
builder.UseSplash<MeuSplashView, MeuSplashViewModel>();
```

---

## Executando as Demonstrações e Testes

O repositório inclui aplicações de demonstração completas em **Avalonia UI** e **WPF** compartilhando o mesmo projeto `Caramelo.MvvmApp.Demo.Core`.

### Executar Demo Avalonia (Multiplataforma: Windows, Linux, macOS)
```bash
dotnet run --project src/Caramelo.MvvmApp.Avalonia.Demo/Caramelo.MvvmApp.Avalonia.Demo.csproj
```

### Executar Demo WPF (Windows)
```bash
dotnet run --project src/Caramelo.MvvmApp.WPF.Demo/Caramelo.MvvmApp.WPF.Demo.csproj
```

### Executar os Testes Unitários
```bash
dotnet test src/Caramelo.MvvmApp.Tests/Caramelo.MvvmApp.Tests.csproj
```

---

## Licença

Distribuído sob a licença **MIT**. Consulte o arquivo [LICENSE](LICENSE) para mais informações.
