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

## Como executar

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

## Compatibilidade

O projeto utiliza WPF e tem como alvo `net10.0-windows`. Portanto, sua execução nativa é destinada ao Windows.

A propriedade `EnableWindowsTargeting` permite que o projeto seja compilado em ambientes que não sejam Windows com os componentes de targeting apropriados, mas isso **não torna o WPF multiplataforma**. A execução em Linux depende de uma camada de compatibilidade, como Wine, e não é garantida pelo projeto.

## Fontes de dados e créditos

Os dados estruturados de Pokémon, movimentos, habilidades, evoluções e sprites são organizados em arquivos de seed versionados no repositório. A documentação de persistência do projeto identifica a [PokéAPI](https://pokeapi.co/) e o repositório de [sprites da PokéAPI](https://github.com/PokeAPI/sprites) como fontes utilizadas na preparação desses dados.

Pokémon e suas marcas, personagens e recursos visuais pertencem aos respectivos titulares. Este projeto é um trabalho independente, sem associação oficial com a The Pokémon Company, Nintendo, Game Freak ou Creatures.

---

**Repositório:** [Yukitto-dark/Gestao-de-Pokmon-](https://github.com/Yukitto-dark/Gestao-de-Pokmon-)
