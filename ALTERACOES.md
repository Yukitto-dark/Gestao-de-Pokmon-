# GestaoDePokemon

Versão que une as funcionalidades da branch `alt` (Time, build do Pokémon, rotas, shiny no
time e nos favoritos, linha evolutiva completa) aos dados de todas as regiões da branch `master`.
As seções a partir de "Código" registram o histórico da branch `alt` (escopo Gen III); a seção
"Integração com a master" no fim descreve o estado atual.

## Dados

- 1025 espécies (#001–#1025), 10 regiões (Kanto a Paldea e Hisui) e 181 rotas numeradas.
- 1525 formas (Normal, regionais, Megas, Gigantamax e demais formas), 834 golpes, 313 habilidades.
- Banco: `%LOCALAPPDATA%\GestaoDePokemon\gestaopokemon.db` (o mesmo da `master`, migrado na abertura).

## Código

- `AppDbContext.cs`: nome do banco alterado para `gestaopokemon-gen3.db`.
- `DatabaseInitializer.cs`: apenas as regiões 1–3; removidos os seeders de Kyurem,
  formas/megas Gen VI–IX, evoluções por forma, learnsets Gen VII+ e megas.
- `GameLearnsetsInitializer.cs`: jogos principais limitados a Red/Blue/Yellow,
  Gold/Silver/Crystal e Ruby/Sapphire/Emerald.
- `PokemonMeasurementsInitializer.cs`: espera 386 espécies e 33 formas alternativas.
- `Box.xaml` / `Box.xaml.cs`: filtro e seção de mega evolução removidos.
- `View/fonts/pokemon_fire_red.ttf`: 23 glifos acentuados criados na própria fonte (`ç`, `ã`, `õ`, `á`, `é`, etc.), mantendo o mesmo desenho e largura das letras base.
- `Regioes.xaml` / `Regioes.xaml.cs`: removidos o bloco “Mapa” das informações da rota e os botões/numeração de paginação; abas de regiões voltaram ao layout anterior.
- `Box.xaml`: textos do painel de detalhes unificados em tons de azul (sem branco/sombra); estilo `HotspotTipo` agora aplica a sobreposição escura por cima do conteúdo, fazendo o botão de tipo Fada escurecer ao ser selecionado como os demais.
- `Box.xaml.cs`: linha evolutiva obtida da cadeia completa (todas as etapas, inclusive em evoluções de três etapas); seção “Atributos por forma” e tooltips com atributos base, intervalo em Lv.100, altura, peso, tipos e habilidades de cada forma.
- `PokemonSqliteRepository.cs` / `IPokemonRepository.cs` / `BoxController.cs`: nova consulta `ObterLinhaEvolutiva`, que percorre ancestrais e descendentes com visited set para evitar repetições/ciclos.
- `PokemonFavorite.cs` / `PokemonPartyMember.cs`: coluna `IsShiny` para registrar se o Pokémon foi favoritado/colocado no time com o sprite shiny ativo.
- `DatabaseInitializer.cs`: `EnsureShinyColumns` adiciona `IsShiny` a bancos já existentes sem apagar registros.
- `FavoritosSqliteRepository.cs` / `PartySqliteRepository.cs` / versões em memória e contratos: `EhShiny` e `Adicionar`/`Alternar` com o estado shiny.
- `PokemonSqliteRepository.cs`: `ObterSpriteUrl(id, shiny)` passa a usar `BoxShiny` (`miniShiny`) quando solicitado.
- `Box.xaml.cs`: ao selecionar um Pokémon salvo como shiny, apenas `Imagem_Pokemon` abre em `frontShiny`; favoritar e adicionar ao time gravam o estado do botão Shine. As miniaturas da grade (`_1` a `_30`) continuam sempre normais.
- `TimeController.cs` / `Time.xaml.cs`: time expõe `(Id, Shiny)` e carrega a miniatura shiny do membro, com tooltip indicando `#id (shiny)`.
- `Regioes.xaml`: `Informacoes_Rota` passou a usar `/View/fonts/#Pokemon Fire Red`, igual aos demais textos da tela.
- `Pokeset.xaml` / `Pokeset.xaml.cs` / `PokesetController.cs`: tela antiga removida; substituída pelo popup de set sobre a tela de Time.
- `View/Controles/PokemonSetPopup.xaml` / `.xaml.cs`: popup com abas Resumo/Stats/Moves e texturas Pixelmon (`summarysummary.png`, `summarystats.png`, `summarymoves.png`). Edita nível, natureza, habilidade, 6 IVs, 6 EVs e até 4 movimentos. Painel reduzido para 520×304 (80% do tamanho original), com sobreposição cinza clara no lugar do retângulo preto a 70% e sombra nos textos para manter a legibilidade.
- `Controller/PokemonSetController.cs`: leitura e gravação do set no membro do time, validando habilidade e movimentos pela forma Normal; movimentos disponíveis consultados diretamente em `LearnableMoves`.
- `PokemonPartyMember.cs` / `AppDbContext.cs` / `DatabaseInitializer.cs` (`EnsureSetColumns`): colunas de set no time (Level, Nature, AbilityId, IVs, EVs e MoveIdsJson), com migração leve para bancos existentes.
- `Time.xaml` / `Time.xaml.cs`: miniaturas do time abrem o popup de set sobre a própria tela, sem navegar para outra Page.
- `Data/images/pokemon/miniShiny/#001–#386.png`: canvas reduzido de 68×56 para 40×30, com o conteúdo realocado para a mesma posição da sprite `mini` correspondente. Antes, o espaço transparente extra fazia a sprite shiny renderizar cerca de 40% menor que a normal no Time; agora ambas ocupam o mesmo tamanho na tela.

## Seeds

Os arquivos em `Data/Persistence/Seed` foram reduzidos para o escopo Gen III:
pokemon (386), moves (353), abilities (82), learnsets (75.796 entradas; 76.443 no banco após as formas),
evoluções (194), medidas (419), formas alternativas (33) e nenhuma mega.

## Validação

- `dotnet build GestaoDePokemon.slnx`: 0 avisos, 0 erros.
- App iniciado sem exceções; banco gerado com as contagens acima.
- Smoke test do popup: abertura clicando em `Party_1`, edição de nível/IV e seleção de movimento persistidos no SQLite; fechamento sem navegação. Screenshot conferido: fundo do popup em cinza claro (brilho médio ~189) e painel centralizado em 520×304.
- Ajuste de posições após análise de pixels: linhas do Grid passaram de alturas relativas (`*`) para fixas (36/210/28), evitando que a métrica da fonte Pixel Fire Red empurrasse os botões; `LineHeight` 13, labels com mais respiro e botões de 26 px centralizados. Medição por automação confirmou IV/EV e os quatro moves alinhados às colunas do cabeçalho.
- `PokemonSetPopup.xaml`: estilo visual trocado para o padrão da referência — moldura azul-clara com gradiente (`#D3DCEB` → `#B0C0D0` → `#A9BDCD`), fundo dos painéis `#303030`, rodapé dos botões `#222222` e texto branco `#F8F8F8` sem sombra. Texturas Pixelmon e retângulo clareador removidos; `MostrarAba` não troca mais a imagem de fundo. Margem interna ajustada de 13 para 10 para compensar a moldura de 5 px e evitar clipping do rodapé.
- `View/Telas - background`: adicionadas `Gui-Summary.png` (827×657), `Gui-Status.png` (703×559) e `Gui-Moves.png` (895×664); `Favoritos.png` renomeada para `Time.png`.
- `View/Telas/Time.xaml`: imagem de fundo passou de `Favoritos.png` (inexistente após a renomeação) para `Time.png`.
- `View/Controles/PokemonSetPopup.xaml` / `.xaml.cs`: popup de edição substituído por um host que troca de página pela barra inferior. Clicar em qualquer `Party_*` abre a página de Resumo; as quatro áreas de clique invisíveis da barra mapeiam Resumo, Status, Moves e Fechar. `Viewbox` centraliza e escala a página ativa conforme seu tamanho nativo. O evento `Fechado` e a assinatura `Abrir(int, bool)` foram mantidos para compatibilidade com `Time.xaml.cs`.
- `View/Controles/GuiSummaryControl.xaml` / `GuiStatusControl.xaml` / `GuiMovesControl.xaml` (com seus `.xaml.cs`): três UserControls separados, um por imagem Gui (`827×657`, `703×559` e `895×664`). Cada um traz a imagem de fundo e um `Grid x:Name="Camada"` vazio por cima para receber labels e botões, com `d:DesignWidth`/`d:DesignHeight` iguais ao tamanho nativo da imagem para edição confortável no designer.
- Snapshot `v1` criado em `C:\Users\User\OneDrive\Área de Trabalho\v1` antes de alterar o projeto principal (sem `bin`, `obj` ou `.vs`).
- `View/Controles/GuiSummaryControl.xaml.cs`: botões `Ir_stats`, `Ir_Moves`, `Voltar_Time` e a imagem clicável `Ir_Move` ligados à navegação/fechamento. Labels preenchidos: `Id_pokemon`, `lvl_pokemon`, `Nome_pokemon`, `Imagem_pokemon`, `Vida` (HP máximo calculado por nível/IV/EV/base), `Ability` e `Info_ability`. `Experiencia` e `XP_faltando` mostram `—` porque o modelo não guarda XP.
- `View/Controles/GuiStatusControl.xaml.cs`: botões `Ir_summary`, `Ir_moves`, `Voltar_Time` ligados. Labels preenchidos: `Id_pokemon`, `lvl_pokemon`, `Nome_pokemon`, `Imagem_pokemon`, `Type_pokemon`, os seis `IV_*`, `Nature_pokemon`, `Size_A` (altura) e `Size_subA` (peso). `happiness_pokemon` mostra `—` porque não há felicidade persistida; `Nome_trainer` permanece fixo.
- `View/Controles/GuiMovesControl.xaml.cs`: botões `Ir_summary`, `Ir_stats`, `Voltar_Time` ligados. Quatro nomes, PPs, ícones de tipo e os labels de poder/precisão/categoria/descrição são preenchidos a partir dos movimentos salvos; se o set não tiver movimentos, usa os quatro primeiros disponíveis na forma Normal. Clicar em um nome de movimento atualiza os detalhes.
- `View/Controles/GuiDados.cs`: helper compartilhado para obter a forma Normal, resolver sprites e ícones de tipo, calcular HP máximo e separar `#número` do nome.
- `PokemonSetPopup.xaml.cs`: `Abrir(int, bool)` agora carrega o `SetInfo` do `PokemonSetController` e entrega aos três controles; se não houver membro no time, exibe aviso e não abre.
- `AppDbContext.cs` / `model/ValueObjects/IndividualValues.cs`: correção da leitura dos IVs salvos. O `JsonSerializerOptions` do contexto passou a usar `PropertyNameCaseInsensitive` e `IndividualValues` recebeu `[JsonConstructor]`, porque o struct tem propriedades `PascalCase` e parâmetros `camelCase`; sem isso, o banco com IVs 31 era lido como 0.

## Validação adicional

- `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.
- Teste direto do controller com `#282`: IVs lidos como `31,31,31,31,31,31` (antes `0,0,0,0,0,0`).
- Smoke test de automação: `Party_1` abre o Resumo com `#282 Gardevoir`, `Lv100`, `277/277` de HP, habilidade `Synchronize` e descrição; Status mostra os seis IVs em 31, altura `1.6 m`, peso `48.4 kg` e natureza `Hardy`; Moves mostra quatro movimentos com PP/power/precisão/categoria.
- Smoke test de navegação: `Ir_stats` → Status, `Ir_moves` → Moves, `Ir_summary` → Resumo e `Voltar_Time` fecha o popup sem sair da tela Time.
- Snapshot `v2` criado em `C:\Users\User\OneDrive\Área de Trabalho\v2` antes desta rodada de ajustes visuais (sem `bin`, `obj` ou `.vs`).
- `GuiSummaryControl.xaml`, `GuiStatusControl.xaml`, `GuiMovesControl.xaml`: `FontFamily="/View/fonts/#Pokemon Fire Red"` aplicado no `UserControl`, herdado por todos os labels e botões. Botões de navegação (e o hotspot `Ir_Move`) com `Opacity="0"`, permanecendo clicáveis.
- `GuiSummaryControl.xaml`: adicionados `Type_pokemon` e `Type_pokemon2` acima de `Imagem_pokemon` (posição antes ocupada apenas pelo hotspot invisível).
- `GuiStatusControl.xaml` / `GuiMovesControl.xaml`: `Type_pokemon` redimensionado de `173×23` / `217×39` para `78×32` (proporção correta dos ícones) e adicionado `Type_pokemon2`.
- Alinhamento padrão nas três telas: os dois ícones de tipo têm `78×32`, ficam lado a lado com 4 px de separação, alinhados à esquerda da sprite e 7–8 px acima dela.
- `GuiDados.cs`: novo `Tipos(Image, Image, IReadOnlyList<PokemonType>)`, que preenche um ou dois ícones e colapsa o segundo quando o Pokémon tem só um tipo.
- `GuiSummaryControl.xaml.cs` / `GuiStatusControl.xaml.cs` / `GuiMovesControl.xaml.cs`: uso de `GuiDados.Tipos` com `form.Types`, substituindo o código que mostrava apenas o primeiro tipo (Resumo não mostrava nenhum).
- Validação por automação: #282 (Psychic/Fairy) mostrou os dois ícones nas três telas; retângulos medidos — Resumo `Type_pokemon` em `X699 Y374 57×23` e sprite em `X699 Y404 132×131`; Status `X690 Y349 67×26` e sprite `X690 Y382 140×139`; Moves `X684 Y372 56×22` e sprite `X684 Y401 136×136`. Navegação e fechamento continuam funcionando com os botões invisíveis.
- Snapshot `v3` criado em `C:\Users\User\OneDrive\Área de Trabalho\v3` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- `GuiSummaryControl.xaml`, `GuiStatusControl.xaml`, `GuiMovesControl.xaml`: estilo de label igual ao de `Numeração_Box` — `Pokemon Fire Red`, branco, `HorizontalContentAlignment`/`VerticalContentAlignment` em `Center` e `DropShadowEffect` preto (`ShadowDepth=3`, `Direction=315`, `BlurRadius=0`). Tamanhos adaptados ao slot (`24` em id/nível, `22` no nome, `20` no padrão, `18`/`17`/`16`/`14` nos textos menores). `Info_ability` e `Move_description` continuam alinhados à esquerda/topos.
- Alinhamento padronizado nas três telas: `Id_pokemon` em `sprite.X + 28`, `lvl_pokemon` em `sprite.X + 160`, ambos em `Y=20` com `52×33`; `Nome_pokemon` começa no mesmo X da sprite, `22 px` abaixo dela, com a largura da sprite e altura `34`.
- `GuiDados.Tipos`: agora recebe a posição/largura da sprite e centraliza o bloco de tipos — com dois tipos, o par `78+4+78` é centrado; com um tipo, o ícone único é centrado e o segundo fica `Collapsed`. A visibilidade é definida antes de carregar a imagem.
- Validação por leitura de pixels: diferença entre a captura e o background original usada para isolar o texto branco. Todos os labels ficaram centralizados dentro do próprio bloco (desvio máximo de 1–3 px), exceto `Move_description`, alinhado à esquerda por ser texto longo, e `Nature_pokemon` com +3 px verticais.
- Teste do helper de tipos com sprite `X56 L180`: um tipo → primeiro em `X107`, segundo oculto; dois tipos → primeiro em `X66`, segundo em `X148`, ambos visíveis.
- Build final: 0 avisos, 0 erros. Navegação e fechamento revalidados.
- Snapshot `v4` criado em `C:\Users\User\OneDrive\Área de Trabalho\v4` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- `GuiSummaryControl.xaml`: `Info_ability` estava dentro de um `Label` wrapper e ambos herdavam o mesmo `DropShadowEffect` do estilo implícito, gerando duas sombras (a segunda exibia o texto e a própria sombra). O wrapper recebeu `Effect="{x:Null}"`, preservando geometria e alinhamento; a sombra única continua no `Info_ability`.
- Verificação A/B por pixels da região do `Info_ability`: sem a correção, pixels escuros apareciam a 6–9 px do texto branco (contagens 1316–2754); com a correção, esses deslocamentos caem para 158–218 (ruído de antialiasing) e permanece apenas a sombra a 2–3 px. 2204 pixels mudaram apenas na faixa da segunda sombra.
- `Id_pokemon` e `lvl_pokemon` agora mostram somente o número nas três telas: `GuiDados.Id` usa `TrimStart('#')` (#282 → 282) e os três code-behind passaram de `$"Lv{info.Level}"` para `info.Level.ToString()` (Lv100 → 100).
- Automação confirmou nas três páginas: `Id_pokemon|282` e `lvl_pokemon|100`. Build final: 0 avisos, 0 erros.
- `Info_ability` e `Move_description` viraram `TextBlock` (em vez de `Label`), com `TextWrapping="Wrap"`, `FontFamily` Pokémon e o mesmo `DropShadowEffect`. O code-behind passou de `.Content` para `.Text`.
- O popup usa `Viewbox Stretch="Uniform"`, que mede os filhos com largura infinita e impede a quebra automática do `TextBlock`. Por isso `GuiDados.QuebrarLinhas(texto, maximoPorLinha)` insere `\n` em limites de palavra: 55 caracteres no Resumo (fonte 18) e 27 em Moves (fonte 35).
- `Info_ability` teve a fonte reduzida de 30 para 18: a descrição de `Synchronize` tem 85 caracteres (656 px medidos com a fonte Pokémon), não cabendo em uma linha de 445 px. Com a quebra, ficaram 2 linhas completas sem corte (faixas de pixels y439–447 e y450–456).
- `Move_description` com `"Attract · Normal · Status"` continua em uma linha e não é mais cortado.
- Validação por pixels e navegação entre as três telas reconfirmadas. Build final: 0 avisos, 0 erros.

## Edição de EVs

- Snapshot `v5` criado antes da primeira tentativa (edição de IVs), revertida a pedido; snapshot `v6` criado em `C:\Users\User\OneDrive\Área de Trabalho\v6` antes da troca para EVs (sem `bin`, `obj` ou `.vs`).
- Regra aplicada (Geração III+): cada EV aceita de `0` a `252` (`EffortValues.MaxPerStat`) e a soma das seis estatísticas não pode passar de `510` (`MaxTotal`). O rodapé mostra `EVs disponíveis: 510 − total`.
- `IndividualValues.cs` voltou ao estado original: nenhuma alteração de IV foi mantida.
- `PokemonSetController.cs`: novo `AtualizarEV(int, PokemonStat, int)`, que usa `EffortValues.Set` (valida o teto por estatística e o total) e grava apenas as colunas `EV_*`, sem mexer em IVs, nível, natureza, habilidade ou movimentos.
- `GuiStatusControl.xaml` / `.xaml.cs`: labels renomeados para `EV_*` e preenchidos com `info.EVs`. Clicar no valor de um EV ou no nome da estatística no fundo da imagem (hotspot invisível) abre um `TextBox` no mesmo lugar; `Enter` confirma, `Esc` cancela e a perda de foco confirma. Valores fora de `0..252` ou que estourariam os `510` totais são recusados com aviso; a gravação é imediata no SQLite.
- `PokemonSetPopup.xaml.cs`: o evento `EVsAlterados` recarrega as três páginas com o `SetInfo` atualizado e mantém a página atual. O HP do Resumo é recalculado junto.
- `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.
- Smoke test de automação: `EV_hp` 0 → 100 (saldo 510 → 410), clique no rótulo de `Attack` 0 → 200 (saldo 410 → 210), `300` recusado pelo limite por estatística e `250` recusado pelo limite total; Resumo passou a `286/286` de HP. SQLite confirmou `EV_HP=100` e `EV_Attack=200` para `#282`. Os valores alterados pelo teste foram restaurados depois (IVs em 31 e EVs em 0).

## Edição de nível e natureza

- Snapshot `v7` criado em `C:\Users\User\OneDrive\Área de Trabalho\v7` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- `PokemonSetController.cs`: novos `AtualizarNivel(int, int)` (valida `1..100`) e `AtualizarNatureza(int, PokemonNature)` (valida o enum), ambos gravando só a coluna alterada.
- `View/Controles/EdicaoInline.cs`: helper compartilhado que transforma um `Label` em campo clicável. Criado para o nível e usado nas três páginas (Resumo, Status e Moves), mantendo o mesmo padrão dos EVs (`Enter` confirma, `Esc` cancela, perda de foco confirma).
- `GuiStatusControl.xaml` / `.xaml.cs`: clique em `Nature_pokemon` mostra o `ComboBox` `Nature_select` no mesmo lugar, com as 25 naturezas do enum — equivalente a um `<select>` HTML. A escolha salva imediatamente no `SelectionChanged`; `DropDownClosed` fecha/cancela se nada foi escolhido. Estado inválido (`101`) e natureza fora do enum são recusados.
- `GuiSummaryControl.xaml.cs` e `GuiMovesControl.xaml.cs`: `lvl_pokemon` também ficou editável por clique, com o mesmo helper e validação.
- `PokemonSetPopup.xaml.cs`: as três páginas passaram a expor `SetAlterado`; qualquer alteração recarrega o `SetInfo` e mantém a página atual, recalculando o HP do Resumo.
- `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.
- Smoke test de automação: nível 50 → 60 no Resumo (HP `143/143` → `170/170`), natureza `Jolly` → `Adamant` direto pelo dropdown e nível `101` recusado com aviso. SQLite confirmou `Level=60` e `Nature='Adamant'`; os valores de teste foram restaurados depois para `Level=100` e `Nature='Hardy'`.

## Golpes editáveis, mínimo de EVs e efeito da natureza

- Snapshot `v8` criado em `C:\Users\User\OneDrive\Área de Trabalho\v8` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- Pesquisa (Bulbapedia/PokeAPI): não existe EV mínimo oficial — todo Pokémon começa com 0. A pedido, adotou-se como mínimo o **EV yield da espécie**. A natureza **não altera EVs**: ela aplica +10%/−10% sobre o **stat final** (inclusive sobre os pontos gerados pelos EVs), nunca sobre o HP. O projeto já tinha `PokemonNatureRules`/`NatureModifiers` definidos e sem uso; agora são exibidos na tela de Status.
- `model/ValueObjects/EvYield.cs` e `model/Rules/PokemonEvYields.cs`: tabela estática com o `effort` das 386 espécies Gen III, extraída do CSV oficial `data/v2/csv/pokemon_stats.csv` do PokeAPI. Ex.: `#282` → `Sp.Atk 3`, `#006` → `Sp.Atk 3`, `#009` → `Sp.Def 3`.
- `PokemonSetController.cs`: `Obter` normaliza os EVs para o mínimo da espécie e grava se necessário; `AtualizarEV` rejeita valores abaixo do mínimo; `Salvar` usa o mínimo como limite inferior. `SetInfo` passou a expor `Yield` e `TodosOsMoves` (catálogo inteiro de 353 golpes, sem filtro de nível/learnset).
- `AtualizarMove(int pokemonId, int slot, int? moveId)`: salva qualquer golpe existente no catálogo nas quatro posições; `null` limpa o slot. Não há filtro por nível.
- `GuiMovesControl.xaml` / `.xaml.cs`: cada `Move_name*` virou um `<select>` (`Move_select1..4`) com 354 opções ("—" + 353 golpes). Clique no nome abre o dropdown no mesmo lugar; `SelectionChanged`/`DropDownClosed` confirmam. O detalhe (Poder, Acurácia, Categoria, descrição) é atualizado na escolha, e a página é recarregada mantendo o slot.
- `GuiStatusControl.xaml` / `.xaml.cs`: novo rótulo `Nature_effect` (`+Ataque / -Sp.Atk`, por exemplo), preenchido via `GuiDados`. O rótulo `EV_minimos` chegou a ser incluído nesta rodada, mas foi removido a pedido na rodada seguinte.
- `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.
- Smoke test de automação: `Mín. espécie: Sp.Atk 3` aplicado a `#282` (`EVs disponíveis: 507`); tentativa de zerar `Sp.Atk` recusada; dropdown de golpes com 354 itens e `Fire Blast` salvo no slot 1 (`MoveIdsJson='[126]'`, PP `5/5`); natureza `Adamant` exibiu `+Ataque / -Sp.Atk`. Teste restaurado para `Nature='Hardy'`, `MoveIdsJson='[]'` e `EV_SpecialAttack=3`.

## EVs começando no mínimo, sem aviso na tela

- Snapshot `v9` criado em `C:\Users\User\OneDrive\Área de Trabalho\v9` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- A pedido, o resumo `Mín. espécie: ...` foi removido da aba Status. Os EVs continuam sendo normalizados para o mínimo do EV yield ao abrir o popup (`GarantirEVsMinimos`) e `AtualizarEV` continua rejeitando valores abaixo do mínimo — apenas a informação visual foi removida.
- `GuiStatusControl.xaml.cs`: removida a atribuição a `EV_minimos`, que ainda referenciava o rótulo já excluído do XAML e deixava a compilação com `CS0103`.
- `GuiDados.cs`: removido o helper `MinimosEspecies`, sem uso após a mudança. `Nature_effect` e o restante da edição permanecem intactos.
- `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.
- Smoke test de automação: com `EV_SpecialAttack=0` no SQLite, ao abrir `Party_1` o campo exibiu `3` e o saldo `507`; nenhum rótulo/texto de mínimo apareceu na tela. O banco ficou em `EV_SpecialAttack=3`.

## Correções de golpes e mínimo fixo de EVs

- Snapshot `v10` criado em `C:\Users\User\OneDrive\Área de Trabalho\v10` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- Golpes padrão preservados: `GuiMovesControl` agora guarda os quatro slots exibidos (`_moveIdsAtuais`) e, ao alterar um deles, envia a lista completa ao controller. Assim, trocar só o primeiro golpe não apaga os outros três. Os golpes padrão (`Absorb`, `Acid`, `Acid Armor`, `Aerial Ace`) passam a ser gravados em `MoveIdsJson` na primeira edição.
- Sem golpes repetidos: o dropdown de cada slot oculta golpes já usados nos outros três (mantendo o golpe atual do próprio slot). `AtualizarMoves(int, IReadOnlyList<int>)` valida duplicatas e golpes inexistentes, e `GarantirMovesUnicos` normaliza duplicatas já presentes no banco ao abrir o popup.
- `AtualizarMove` foi substituído por `AtualizarMoves`, que sempre salva os quatro slots de uma vez (com `0` para vazio e remoção apenas de zeros finais).
- Mínimo de EVs passou a ser fixo em **10 para todas as espécies** (`EffortValues.MinPerStat`). `GarantirEVsMinimos` completa até 10 e, se necessário, reduz os maiores valores para respeitar o teto de 510; `AtualizarEV` e `Salvar` também usam 10 como piso.
- Removidos `EvYield.cs` e `PokemonEvYields.cs`, além de `SetInfo.Yield`, que não fazem mais sentido com o mínimo fixo.
- `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.
- Smoke test de automação: com os EVs em 0, o popup abriu com `10` em todos os atributos e saldo `450`; tentar `5` em `EV_hp` foi recusado. Trocar o slot 1 para `Fire Blast` manteve `Acid`, `Acid Armor` e `Aerial Ace` nos demais (`MoveIdsJson='[126,51,151,332]'`). O dropdown do slot 2 tinha 351 itens e não oferecia `Fire Blast` nem `Acid Armor`. Duplicatas presentes no banco foram normalizadas na abertura. Estado final restaurado: `#282` com `Nature='Hardy'`, EVs `10`, `MoveIdsJson='[]'`; `#6` limpo para `'[51]'`.

## Remoção do filtro por atributos

- Snapshot `v11` criado em `C:\Users\User\OneDrive\Área de Trabalho\v11` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- O painel "Atributos" foi removido de `Box.xaml` (botão `Por_Atributos`, `Painel_Atributos`, `Filtro_Stat`, `Stat_Min`, `Stat_Max`). Os demais filtros (tipo, região, favoritos, time, habilidade, formas, id, nome) permanecem.
- `Box.xaml.cs`: removidos a inicialização do combo de estatísticas, o handler `Por_Atributos_Click`, a limpeza dos campos de atributo, as validações de mínimo/máximo e o envio de `Stats` ao `PokemonFilter`.
- Removidos `PokemonFilter.Stats`, `Data/Filters/StatRange.cs` e, no repositório SQLite, o tratamento de `Stats` em `RequerDetalhes`, `Corresponde` e o helper `CorrespondeStat`.
- `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.

## Limpeza de código morto e duplicação

- Snapshot `v12` criado em `C:\Users\User\OneDrive\Área de Trabalho\v12` antes desta rodada (sem `bin`, `obj` ou `.vs`).
- `PokemonSetController`: removidos `SetInfo.Moves`, o `ThenInclude` de learnset que só o alimentava, o método `Salvar` sem chamador e a formatação `#000 Nome` reparseada pela view. `ObterForma` passou a ser o helper único, usado também por `GuiDados`.
- `SetInfo.Nome` agora contém apenas o nome da espécie; `GuiDados.Id` formata o número a partir de `PokemonId`. Removido o reparseio em `GuiDados.Id/Nome`.
- Normalização centralizada: `EffortValues.GarantirMinimo()` concentra o piso 10 e o teto 510; `PokemonSetController.RemoverZerosFinais()` concentra o ajuste dos slots de golpes; `EffortValues.Estatisticas` é a única lista de atributos, usada pelo controller e por `GuiStatusControl`.
- `BoxController` passou a cachear os IDs da página e a invalidar o cache ao trocar filtro, página, favorito ou time.
- Removidos: filtro `HasMegaEvolution`, eventos `Alterado` sem assinantes, `IPartyRepository.Limite`, API legada `Total`/`ObterIds(pagina,tamanho)`, `RotaInfo.MapaImagem`, `IndividualValues.GenerateRandom`, `MapasController`, entidades `PokemonTeam`/`PokemonBuild` e seus mapeamentos no `AppDbContext`, repositórios `InMemory`, inicializadores não chamados, seeds/reports Gen VI–IX, backup `.bak` e imagens/fonte sem referência.
- Os inicializadores `RegionGamesInitializer` e `BoxIconsInitializer`, que estavam no mesmo arquivo de `GameLearnsetsInitializer`, foram preservados em arquivos próprios.
- `.csproj` limpo: removidos `<Folder Include>`, glob de `Tipagem`, backgrounds `PokeSet*` inexistentes e referências a seeds apagados. READMEs de persistência, filtros e sprites atualizados; referências a scripts inexistentes removidas.
- Removidos `x:Name` sem uso nos três controles do popup e comentários de template com `.xam`.
- `dotnet clean` + `dotnet build GestaoDePokemon.slnx -v minimal`: 0 avisos, 0 erros.
- Smoke test de automação: filtro por atributos ausente na Box, filtros de habilidade/id presentes; `Party_1` abriu o popup sobre o Time com `Gardevoir`, EV `10`, saldo `450` e golpes `Absorb`/`Acid`. Banco SQLite restaurado ao estado original após o teste.

## Integração com a master (todas as regiões)

- Base de código da `alt` com os seeds e inicializadores completos da `master`: `KyuremFormsInitializer`,
  formas das gerações I–IX (`generation-i-v`, `generation-vi`, `generation-vii-ix`), Mega Evoluções
  (`generation-vi-megas`, `generation-ix-megas`, `pokemon-mega-evolutions`), `EvolutionFormsInitializer`,
  `MegaEvolutionsInitializer` e `GameLearnsetsInitializer` (golpes da Geração VII em diante).
  `RegionGamesInitializer` voltou a listar os jogos de todas as regiões e `SeedRegions` cadastra as 10 regiões.
- Os caminhos de sprite dos seeds da `master` foram convertidos de `View/images/pokemon` para
  `Data/images/pokemon`. `MigrateLegacySpritePaths` faz o mesmo em bancos criados pela `master`.
- `AppDbContext`: banco `gestaopokemon.db` (o da `master`). As rotinas `Ensure*` da `alt` acrescentam
  rotas, time, colunas de shiny e de set a esse banco sem apagar favoritos.
- Box com a ordenação da `master`: cada célula é uma forma, em ordem de Pokédex, com a forma Normal
  primeiro e as demais em ordem de cadastro (`PokemonBoxEntry`, `IPokemonRepository.ObterEntradas`).
  O `BoxController` guarda a lista filtrada e pagina em memória. Os filtros da `alt` continuam:
  tipo e habilidade valem para cada forma; número, nome, região e favoritos valem para a espécie;
  "Apenas Time" mostra as formas que estão no time. O filtro de região ganhou Sinnoh, Unova, Kalos,
  Alola, Galar, Hisui e Paldea.
- Painel de detalhes da Box mostra a forma selecionada (tipos, habilidades, atributos, medidas e golpes;
  formas sem learnset próprio, como as Megas, usam o da forma Normal). `Obter` usa consulta dividida
  (`AsSplitQuery`) e `ObterLinhaEvolutiva` percorre só os pares de evolução antes de carregar as espécies.
- Time por forma: `PokemonPartyMember.PokemonFormId` (coluna nova, preenchida com a forma Normal em
  registros antigos). O índice único passou de `PokemonId` para `PokemonFormId`, permitindo Vulpix e
  Vulpix de Alola no mesmo time. `IPartyRepository.Membros` devolve `MembroTime` (membro, espécie,
  forma, shiny); o Time e a build usam o ícone, o sprite, os tipos, as habilidades e os golpes da forma.
  Os favoritos continuam por espécie.
- `PokemonSetController` passa a trabalhar pelo id do membro do time e corrige os golpes da build:
  - `GolpesDisponiveis` contém somente os golpes que a forma aprende (nível, TM, tutor, ovo ou evolução,
    em qualquer jogo), com a origem no nome: "Flamethrower (Nv 32)", "Earthquake (TM)". Antes o
    dropdown oferecia o catálogo inteiro.
  - Os níveis vêm do jogo de referência da forma (o mais recente da série principal com golpes por
    nível, ex.: Scarlet/Violet). Sem golpes salvos, a build mostra os quatro últimos golpes aprendidos
    por nível até o nível atual (antes eram os quatro primeiros do catálogo em ordem alfabética).
  - `AtualizarMoves` recusa golpes que a forma não aprende; `GarantirMovesValidos` limpa golpes
    inválidos ou repetidos gravados por versões anteriores.
  - `SetInfo` traz tipos, atributos base, altura e peso da forma; as páginas não consultam mais o banco
    de novo a cada carga (`GuiDados.ObterForma` foi removido).
- Seletor de nível na build (`GuiSummaryControl`, quadro "Level Up"): botões `-`/`+` e uma barra de 1 a
  100. A prévia do nível e do HP é imediata; a gravação acontece quando a barra para (350 ms) e também
  ao fechar o popup ou abrir outro Pokémon. O clique no número do nível continua funcionando nas três páginas.
- Rotas: `rotas-seed.json` ganhou 101 rotas de Sinnoh, Unova, Kalos, Alola e Galar (11.175 encontros),
  geradas dos CSVs da PokéAPI no mesmo formato; as 80 rotas originais não mudaram. `SeedRotas` ficou
  incremental (acrescenta só as rotas que faltam). `RotaSqliteRepository` conhece os nomes de todas as
  versões. A tela de Regiões usa abas compactas em duas linhas quando há mais de quatro regiões e avisa
  quando a região não tem rotas (Paldea e Hisui). A tela de Mapas ganhou atalhos (mesas e mapa da parede)
  para as sete regiões novas.
- `Data/images/pokemon/miniShiny/#387–#1025` e `forms/*/box-shiny.png`: o mesmo ajuste feito antes para
  #001–#386 — canvas de 68×56 reduzido ao tamanho do ícone normal, com o conteúdo alinhado a ele, para
  que o ícone shiny não apareça menor no Time (658 arquivos).
- Validação: `dotnet build GestaoDePokemon.slnx` com 0 avisos e 0 erros. Camada de dados e controllers
  testada em um projeto de console com SQLite real: banco novo (1025 espécies, 1525 formas, 181 rotas,
  nenhum sprite inexistente) e banco criado pelo código da `master` com time antigo (migração dos caminhos,
  do índice e da forma do time, favoritos preservados).

## Correção: falha ao inicializar o banco existente

- Sintoma: "Não foi possível inicializar o banco de dados. An error occurred while saving the entity changes."
  ao abrir com um `gestaopokemon.db` que já existia.
- `rotas-seed.json`: as rotas novas tinham 596 encontros repetidos (mesmos dados em slots diferentes da PokéAPI).
  O seed original não tem nenhum, porque o importador original já descartava repetidos, e tabelas de rotas
  criadas por esse importador podem recusá-los. O gerador agora descarta os repetidos (reproduz 79 das 80 rotas
  originais exatamente) e `SeedRotas` também ignora encontros idênticos na mesma área.
- `DatabaseInitializer`: a importação das rotas deixou de ser obrigatória. Se as tabelas de rotas do banco
  recusarem os dados (esquema criado por outra ferramenta, por exemplo), só essa gravação é desfeita
  (savepoint do EF), o restante é gravado e o aplicativo abre mostrando um aviso com o motivo.
- `DatabaseInitializer`: cada etapa é registrada; um erro agora diz a etapa e a mensagem do SQLite
  ("Falha na etapa \"rotas\": ... table Routes has no column named ..."), em vez da mensagem genérica do EF.
- `App.xaml.cs`: mostra a mensagem completa, grava a exceção inteira em
  `%LOCALAPPDATA%\GestaoDePokemon\erro-banco.txt` e exibe os avisos das etapas opcionais.
- Testado com banco criado pelo código da `master` e tabelas de rotas pré-criadas com restrição única nos
  encontros (importa as 181 rotas) e com esquema de rotas incompatível (abre com aviso; favoritos preservados).

## Correção: aplicativo fechando ao editar um status

- Sintoma: ao confirmar um EV (ou a natureza) na página Status da build, o aplicativo fechava.
- Causa provável: a gravação, a recarga das três páginas e as mensagens de aviso aconteciam dentro dos
  eventos `KeyDown`/`LostFocus`/`SelectionChanged` do próprio controle que estava sendo escondido. Esse tipo de
  reentrância é frágil no WPF e ainda mais no Wine; e, sem tratamento global, qualquer exceção fechava o app.
- `GuiStatusControl`: o editor de EV é escondido no evento, mas a validação, a gravação e a recarga rodam
  depois (`Dispatcher.BeginInvoke`). O mesmo vale para a natureza (`SelectionChanged`/`DropDownClosed`).
  O editor de EV usa a fonte do próprio rótulo, em vez de um `FontFamily` criado em código com caminho relativo.
- `EdicaoInline` (nível digitado) e `GuiMovesControl` (`SelectionChanged` dos golpes): mesma regra de adiar a
  gravação; o `DropDownClosed` dos golpes já era adiado.
- `App.xaml.cs`: `DispatcherUnhandledException` registra o erro em
  `%LOCALAPPDATA%\GestaoDePokemon\erro-app.txt`, mostra a mensagem e mantém o aplicativo aberto.
- Não foi possível reproduzir o fechamento aqui (o Wine não roda no ambiente de testes); se ainda ocorrer
  algum erro, ele aparece na tela e fica registrado em `erro-app.txt`.

## Status da build a partir dos base stats

- Antes, os seis campos da página Status mostravam só os EVs, com mínimo fixo de 10 para todas as espécies
  (10/10/10/10/10/10 para qualquer Pokémon).
- Agora cada campo mostra o base stat da espécie (da forma do membro do time) mais os EVs investidos:
  Garchomp começa em 108/130/95/80/85/102. Editar o campo define o valor do atributo, entre o base stat e
  base + 252; o que é gravado são os EVs acima do base stat. O total continua limitado a 510 EVs e
  "EVs disponíveis" mostra o saldo.
- `EffortValues.MinPerStat` passou de 10 para 0 (o piso agora é o próprio base stat).
- A dica de cada campo mostra base + EVs e o valor real no nível atual (fórmula da Geração III em diante,
  com IV e natureza; HP sem natureza) — `GuiDados.StatNoNivel`.
- `DatabaseInitializer.MigrarEVsPadrao`: membros do time com exatamente 10 EVs nos seis atributos (o mínimo
  automático antigo, nunca editado) voltam a 0. Roda uma única vez por banco (`PRAGMA user_version = 1`).

## Escolha da habilidade na build

- Página Resumo: clicar no nome da habilidade abre uma lista com as habilidades da forma do membro do time,
  na ordem dos slots e com a oculta marcada ("Rough Skin (Oculta)"). A escolha é gravada em
  `PokemonPartyMember.AbilityId` e a descrição é atualizada; a dica do nome mostra as opções.
- `PokemonSetController`: `SetInfo.Habilidades` (habilidade + slot) e `SetInfo.Habilidade` (a escolhida ou,
  sem escolha válida, a primeira); `AtualizarHabilidade` recusa habilidades que a forma não tem (ex.: Vulpix de
  Alola não pode ter Flash Fire).
- O ComboBox segue a regra da natureza: fecha e grava depois que o evento de seleção termina.
