# Gestão de Pokémon

Aplicação desktop de Pokédex desenvolvida em **C#**, **WPF** e **.NET 10**. O projeto reúne um catálogo local de Pokémon, formas alternativas, sprites, habilidades, movimentos, learnsets, evoluções e uma interface inspirada na organização visual dos jogos da franquia.

## Visão geral

O aplicativo utiliza **SQLite** para armazenar os dados da Pokédex e as preferências do usuário. Na inicialização, o sistema executa um inicializador que cria a estrutura necessária e importa os arquivos de dados versionados no próprio repositório.

O catálogo principal contém **1.025 espécies**, da #001 Bulbasaur à #1025 Pecharunt, além de registros de formas alternativas e Mega Evoluções.

## Funcionalidades

### Pokédex / Box

A tela Box apresenta os Pokémon em uma grade paginada de 30 entradas. Cada entrada representa uma forma de uma espécie: a forma Normal aparece primeiro, seguida pelas demais formas cadastradas.

Na seleção de uma entrada, a aplicação exibe o sprite de detalhe correspondente. Quando há um sprite shiny disponível, é possível alternar entre a aparência normal e a shiny.

A Box também permite:

- Filtrar por tipo — os 18 tipos Pokémon.
- Filtrar pela região de origem entre Kanto, Johto e Hoenn.
- Exibir somente Pokémon marcados como favoritos.
- Pesquisar pelo número da Pokédex Nacional.
- Pesquisar pelo nome da espécie.
- Marcar ou desmarcar uma espécie como favorita.

### Favoritos

A tela de Favoritos apresenta os Pokémon selecionados na Box. Os favoritos são persistidos no SQLite e há um limite de **seis espécies**.

### Mapas e regiões

O menu disponibiliza telas para Kanto, Johto e Hoenn, com uma interface para navegar pelas rotas numeradas dessas regiões.

> **Estado atual:** a tela de rotas ainda utiliza um repositório em memória. A listagem numérica está implementada, mas os dados completos de encontros, mapas e informações das rotas ainda não estão conectados à interface por meio do SQLite.

## Dados catalogados

O modelo de dados e os arquivos de seed abrangem:

- **Espécies:** número da Pokédex Nacional, nome, geração, categoria, região de origem e Egg Groups.
- **Formas:** tipos, Base Stats, altura, peso, habilidades, sprites e learnsets específicos por forma.
- **Habilidades:** catálogo de habilidades e associação aos slots das formas.
- **Movimentos:** catálogo de golpes e suas propriedades.
- **Learnsets:** relações entre formas e movimentos, incluindo método de aprendizado, nível e grupo de versões do jogo.
- **Evoluções:** relações entre espécies, com condições e requisitos registrados. O sistema mantém essas informações como dados de consulta; não simula nem decide evoluções automaticamente.
- **Mega Evoluções:** formas Mega cadastradas em conjunto com o relacionamento entre a forma base e a forma Mega, incluindo Mega Pedra ou movimento exigido.

### Modelagem de times

A camada de domínio e persistência também possui entidades para times e configurações de Pokémon. O modelo prevê times de até seis integrantes e configurações com nível, gênero, natureza, IVs, EVs, habilidade, forma e até quatro movimentos, com validações relacionadas aos dados de aprendizado.

**Observação:** na versão atual do menu, ainda não há uma tela de gerenciamento de times. Essa parte está representada no modelo e no contexto de persistência.

## Tecnologias

| Tecnologia | Utilização |
|---|---|
| C# | Linguagem principal |
| .NET 10 | Plataforma e runtime alvo |
| WPF / XAML | Interface gráfica desktop |
| Entity Framework Core 10 | Mapeamento objeto-relacional |
| SQLite | Banco de dados local |
| System.Text.Json | Leitura dos arquivos de seed |

Pacotes principais utilizados pelo projeto:

- `Microsoft.EntityFrameworkCore.Sqlite 10.0.14`
- `Microsoft.EntityFrameworkCore.Design 10.0.14`

## Arquitetura do projeto

O código está organizado para separar a interface, a lógica de aplicação, o acesso aos dados e o modelo de domínio.

```text
GestaoDePokmon-/
├── Controller/                 # Controladores das telas e navegação
├── Data/
│   ├── Configuration/          # Composição dos serviços e repositórios
│   ├── Filters/                 # Critérios de filtragem da Pokédex
│   ├── Persistence/
│   │   ├── Initialization/     # Inicialização do banco e importação dos seeds
│   │   ├── Seed/                # Dados JSON versionados
│   │   └── AppDbContext.cs      # Contexto do Entity Framework Core
│   └── Repositories/
│       ├── Contracts/           # Interfaces e contratos
│       ├── InMemory/             # Repositórios em memória
│       └── SQLite/               # Repositórios persistidos
├── model/
│   ├── Entities/                # Entidades do domínio
│   ├── Enums/                   # Enumerações
│   ├── Rules/                   # Regras do domínio
│   ├── ValueObjects/            # Objetos de valor
│   └── metadata/                # Catálogos e metadados
├── View/
│   ├── Telas/                   # Páginas XAML
│   ├── images/                  # Imagens e sprites
│   ├── fonts/                   # Fontes utilizadas na interface
│   └── cursors/                 # Cursores personalizados
├── App.xaml
├── MainWindow.xaml
├── GestaoDePokemon.csproj
└── GestaoDePokemon.slnx
```

### Persistência

O contexto principal é `AppDbContext`, que configura as entidades e seus relacionamentos com o Entity Framework Core.

Os repositórios são selecionados por `AppServices`. Na configuração atual, os dados da Pokédex e dos favoritos utilizam SQLite; as rotas utilizam o repositório em memória.

O banco é criado no diretório de dados locais do usuário:

- **Windows:** `%LOCALAPPDATA%\GestaoDePokemon\gestaopokemon.db`
- **Wine:** no diretório equivalente a `LocalAppData` dentro do prefixo utilizado.

O inicializador é executado durante a inicialização do aplicativo. Os seeds são importados de forma incremental para evitar duplicar registros existentes. Como o projeto utiliza `EnsureCreated()`, alterações posteriores no modelo precisam de atualização explícita do esquema: `EnsureCreated()` não funciona como um sistema de migrations para bancos já existentes.

## Como executar no Windows

### Requisitos

- Windows com suporte a aplicações WPF.
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- Git.

### Clonar o repositório

```bash
git clone https://github.com/Yukitto-dark/Gestao-de-Pokmon-.git
cd Gestao-de-Pokmon-
```

### Restaurar dependências

```bash
dotnet restore GestaoDePokemon.csproj
```

### Compilar

```bash
dotnet build GestaoDePokemon.csproj
```

### Executar

```bash
dotnet run --project GestaoDePokemon.csproj
```

Na primeira inicialização, o aplicativo cria o banco local e importa os dados disponíveis nos arquivos de seed. O arquivo SQLite fica fora da pasta do repositório, no diretório de dados locais do usuário.

### Publicar uma versão Release

```bash
dotnet publish GestaoDePokemon.csproj -c Release -o ./publish
```

## Como executar no Linux

O projeto utiliza WPF, uma tecnologia de interface gráfica destinada ao Windows. Portanto, **não é possível executar o aplicativo nativamente no Linux**.

Entretanto, é possível compilar o projeto utilizando o SDK do .NET instalado no Linux e executar o aplicativo por meio do **Wine**, que fornece uma camada de compatibilidade para aplicações Windows.

O procedimento abaixo contempla Linux Mint, Ubuntu, Arch Linux e CachyOS.

### 1. Instalar as dependências

#### Debian e Ubuntu

Instale o Git e o SDK do .NET 10:

```bash
sudo apt update
sudo apt install -y git dotnet-sdk-10.0
```

Para instalar o Wine Staging pelo repositório oficial do WineHQ em distribuições baseadas no Ubuntu 24.04, como o Linux Mint 22.x, utilize:

```bash
sudo dpkg --add-architecture i386
sudo apt update

sudo mkdir -pm755 /etc/apt/keyrings

sudo wget -O /etc/apt/keyrings/winehq-archive.key \
    https://dl.winehq.org/wine-builds/winehq.key

. /etc/os-release
UBUNTU_BASE_CODENAME="${UBUNTU_CODENAME:-$VERSION_CODENAME}"

sudo wget -NP /etc/apt/sources.list.d/ \
    "https://dl.winehq.org/wine-builds/ubuntu/dists/${UBUNTU_BASE_CODENAME}/winehq-${UBUNTU_BASE_CODENAME}.sources"

sudo apt update

sudo apt install --install-recommends winehq-staging winetricks
```

O comando utiliza o codinome da distribuição Ubuntu de origem. Em distribuições derivadas do Ubuntu, o valor pode ser diferente do codinome próprio do Linux Mint.

Para outras versões do Ubuntu, consulte as instruções correspondentes no [WineHQ](https://wiki.winehq.org/Ubuntu).

#### Arch Linux e CachyOS

Instale as dependências utilizando o Pacman:

```bash
sudo pacman -Syu
sudo pacman -S --needed dotnet-sdk-10.0 wine winetricks git
```

Caso prefira a versão Staging do Wine, utilize o pacote `wine-staging` no lugar de `wine`.

### 2. Criar um prefixo exclusivo para o aplicativo

É recomendável utilizar um prefixo próprio para manter o ambiente Windows do projeto separado dos demais aplicativos executados pelo Wine.

Crie o prefixo:

```bash
export WINEPREFIX="$HOME/.wine-gestaopokemon"
export WINEARCH=win64

wineboot -u
winecfg
```

O comando `winecfg` abre a configuração do Wine. Aguarde a inicialização do ambiente e feche a janela após a configuração.

### 3. Instalar o .NET Desktop Runtime no Wine

A instalação do SDK do .NET no Linux permite compilar o projeto, mas não fornece o runtime Windows necessário para executar a aplicação WPF.

É necessário instalar o **.NET 10 Desktop Runtime para Windows x64** dentro do prefixo criado anteriormente.

1. Acesse a [página oficial de downloads do .NET 10](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Localize a seção **.NET Desktop Runtime**.
3. Baixe o instalador **Windows x64**.
4. Execute o instalador pelo Wine, mantendo o prefixo configurado:

```bash
export WINEPREFIX="$HOME/.wine-gestaopokemon"

wine "$HOME/Downloads/windowsdesktop-runtime-10.0.X-win-x64.exe"
```

Substitua `X` pela versão do instalador baixado.

Após a instalação, é possível verificar os runtimes disponíveis no prefixo:

```bash
wine "$WINEPREFIX/drive_c/Program Files/dotnet/dotnet.exe" --list-runtimes
```

A lista deverá conter o runtime `Microsoft.WindowsDesktop.App` na versão 10.0.x.

### 4. Clonar o projeto

Com as dependências instaladas, clone o repositório:

```bash
git clone https://github.com/Yukitto-dark/Gestao-de-Pokmon-.git
cd Gestao-de-Pokmon-
```

### 5. Restaurar e compilar

O projeto já possui a propriedade `EnableWindowsTargeting` habilitada no arquivo `.csproj`, permitindo a compilação de aplicações direcionadas ao Windows em outros sistemas operacionais.

Execute:

```bash
dotnet restore GestaoDePokemon.csproj -p:EnableWindowsTargeting=true
```

Compile em Release:

```bash
dotnet build GestaoDePokemon.csproj \
    -c Release \
    -p:EnableWindowsTargeting=true
```

### 6. Executar pelo Wine

Após a compilação, execute o executável gerado:

```bash
export WINEPREFIX="$HOME/.wine-gestaopokemon"

wine ./bin/Release/net10.0-windows/GestaoDePokemon.exe
```

O aplicativo será iniciado utilizando o ambiente Windows criado no prefixo do Wine.

### 7. Localização do banco de dados no Linux

Como o banco utiliza `Environment.SpecialFolder.LocalApplicationData`, sua localização depende do prefixo do Wine.

No prefixo criado neste procedimento, o banco será armazenado em:

```text
~/.wine-gestaopokemon/drive_c/users/<usuario>/AppData/Local/GestaoDePokemon/gestaopokemon.db
```

**Importante:** mantenha o prefixo do Wine para preservar o banco de dados e os dados locais do aplicativo. Excluir o prefixo também remove o banco armazenado dentro dele.

### Observações sobre o Linux

- A compilação é realizada pelo SDK nativo do .NET 10 instalado na distribuição Linux.
- A execução utiliza o Wine e o .NET Desktop Runtime para Windows instalado no prefixo.
- O Wine Staging pode apresentar diferenças de compatibilidade entre versões.
- O projeto não oferece suporte nativo ao WPF no Linux. O funcionamento depende da camada de compatibilidade utilizada.

## Compatibilidade

O projeto utiliza WPF e tem como alvo `net10.0-windows`. Portanto, sua execução nativa é destinada ao Windows.

A propriedade `EnableWindowsTargeting` permite que o projeto seja compilado em ambientes que não sejam Windows com os componentes de targeting apropriados, mas isso **não torna o WPF multiplataforma**. A execução em Linux depende de uma camada de compatibilidade, como Wine, e não é garantida pelo projeto.

## Fontes de dados e créditos

Os dados estruturados de Pokémon, movimentos, habilidades, evoluções e sprites são organizados em arquivos de seed versionados no repositório. A documentação de persistência do projeto identifica a [PokéAPI](https://pokeapi.co/) e o repositório de [sprites da PokéAPI](https://github.com/PokeAPI/sprites) como fontes utilizadas na preparação desses dados.

Pokémon e suas marcas, personagens e recursos visuais pertencem aos respectivos titulares. Este projeto é um trabalho independente, sem associação oficial com a The Pokémon Company, Nintendo, Game Freak ou Creatures.

---

**Repositório:** [Yukitto-dark/Gestao-de-Pokmon-](https://github.com/Yukitto-dark/Gestao-de-Pokmon-)
