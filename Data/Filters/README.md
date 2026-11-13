# Filtros da Box

A tela Box aplica os filtros em conjunto (cada grupo de critérios restringe o resultado dos demais):

- **Tipo:** um único tipo retorna Pokémon com esse tipo em alguma forma. Ao selecionar mais de um tipo, todos os tipos selecionados precisam existir na mesma forma.
- **Região:** marque uma ou mais caixas para mostrar somente Pokémon cuja região de origem seja uma das selecionadas (Kanto OU Johto OU Hoenn). Sem nenhuma caixa marcada, todas as regiões são exibidas. O filtro utiliza `OriginRegionId`, isto é, a região de origem cadastrada para a espécie.
- **Favoritos:** quando ativado, mantém apenas Pokémon presentes em `Favorites` no SQLite.
- **ID:** corresponde exatamente ao número da Pokédex Nacional.
- **Nome:** pesquisa parcial, sem exigir que o texto seja o nome completo.

A paginação é recalculada a cada aplicação dos filtros. O botão Limpar restaura os critérios padrão e retorna à primeira página.

O filtro por tipo é aplicado depois que as formas são materializadas porque `PokemonForm.Types` é armazenado como JSON por um `ValueConverter` do EF Core. Os demais critérios são aplicados diretamente na consulta SQLite.
