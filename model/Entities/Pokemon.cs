using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoDePokemon.model
{
    public class Pokemon
    {
        public int Id { get; set; }

        public int NationalDexNumber { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Generation { get; set; }

        public int OriginRegionId { get; set; }

        public Region OriginRegion { get; set; } = null!;

        public string Category { get; set; } = string.Empty;

        private readonly List<EggGroup> _eggGroups = new();

        public IReadOnlyList<EggGroup> EggGroups => _eggGroups.AsReadOnly();

        private readonly List<Region> _foundInRegions = new();

        public IReadOnlyList<Region> FoundInRegions => _foundInRegions.AsReadOnly();

        public List<PokemonForm> Forms { get; set; } = new();

        /// <summary>Locais/rotas em que esta espécie pode ser encontrada.</summary>
        public List<RoutePokemonEncounter> RouteEncounters { get; set; } = new();

        private readonly List<PokemonEvolution> _evolutions = new();

        public IReadOnlyList<PokemonEvolution> Evolutions => _evolutions.AsReadOnly();

        private readonly List<PokemonEvolution> _preEvolutions = new();

        public IReadOnlyList<PokemonEvolution> PreEvolutions =>
            _preEvolutions.AsReadOnly();

        public void AddEggGroup(EggGroup group)
        {
            if (!Enum.IsDefined(group))
                throw new ArgumentException("Egg Group inválido.", nameof(group));

            if (_eggGroups.Contains(group))
                throw new InvalidOperationException("Esse Egg Group já foi adicionado.");

            bool exclusiveGroup = group is EggGroup.Ditto or EggGroup.Undiscovered;
            bool alreadyHasExclusiveGroup = _eggGroups.Any(g =>
                g is EggGroup.Ditto or EggGroup.Undiscovered);

            if (_eggGroups.Count > 0 && (exclusiveGroup || alreadyHasExclusiveGroup))
                throw new InvalidOperationException(
                    "Ditto e Undiscovered não podem ser combinados com outros Egg Groups.");

            if (_eggGroups.Count >= 2)
                throw new InvalidOperationException(
                    "Um Pokémon não pode possuir mais de dois Egg Groups.");

            _eggGroups.Add(group);
        }

        internal void RegisterPreEvolution(PokemonEvolution evolution)
        {
            if (!_preEvolutions.Contains(evolution))
                _preEvolutions.Add(evolution);
        }

        public void AddEvolution(PokemonEvolution evolution)
        {
            ArgumentNullException.ThrowIfNull(evolution);

            if (evolution.ToPokemon == null)
                throw new ArgumentException("A espécie de destino é obrigatória.", nameof(evolution));

            if (SamePokemon(this, evolution.ToPokemon))
                throw new InvalidOperationException("Um Pokémon não pode evoluir para si mesmo.");

            if (!Enum.IsDefined(evolution.Method))
                throw new ArgumentException("Método de evolução inválido.", nameof(evolution));

            if (evolution.RequiredLevel is < 1 or > 100)
                throw new ArgumentOutOfRangeException(nameof(evolution.RequiredLevel));

            if (evolution.MinimumFriendship is < 0 or > 255)
                throw new ArgumentOutOfRangeException(nameof(evolution.MinimumFriendship));

            if (evolution.MinimumBeauty is < 0 or > 255)
                throw new ArgumentOutOfRangeException(nameof(evolution.MinimumBeauty));

            if (evolution.RequiredTime.HasValue && !Enum.IsDefined(evolution.RequiredTime.Value))
                throw new ArgumentException("Horário de evolução inválido.", nameof(evolution));

            if (evolution.RequiredGender.HasValue && !Enum.IsDefined(evolution.RequiredGender.Value))
                throw new ArgumentException("Gênero de evolução inválido.", nameof(evolution));

            bool alreadyExists = _evolutions.Any(existing =>
                SamePokemon(existing.ToPokemon, evolution.ToPokemon) &&
                existing.Method == evolution.Method &&
                existing.RequiredLevel == evolution.RequiredLevel &&
                existing.RequiredItem == evolution.RequiredItem &&
                existing.RequiredTime == evolution.RequiredTime &&
                existing.MinimumFriendship == evolution.MinimumFriendship &&
                existing.MinimumBeauty == evolution.MinimumBeauty &&
                existing.RequiredGender == evolution.RequiredGender &&
                existing.RequiredLocation == evolution.RequiredLocation &&
                (existing.RequiredMoveId ?? existing.RequiredMove?.Id) ==
                    (evolution.RequiredMoveId ?? evolution.RequiredMove?.Id) &&
                existing.GameVersion == evolution.GameVersion);

            if (alreadyExists)
                throw new InvalidOperationException("Essa evolução já foi cadastrada.");

            evolution.FromPokemon = this;

            evolution.FromPokemonId = Id;

            _evolutions.Add(evolution);

            evolution.ToPokemon.RegisterPreEvolution(evolution);
        }


        public void AddFoundRegion(Region region)
        {
            ArgumentNullException.ThrowIfNull(region);

            if (_foundInRegions.Any(existing => SameRegion(existing, region)))
                throw new InvalidOperationException("Essa região já foi adicionada.");

            _foundInRegions.Add(region);
        }

        public bool RemoveFoundRegion(Region region)
        {
            ArgumentNullException.ThrowIfNull(region);

            Region? existing = _foundInRegions.FirstOrDefault(r => SameRegion(r, region));
            return existing != null && _foundInRegions.Remove(existing);
        }

        private static bool SamePokemon(Pokemon a, Pokemon b) =>
            ReferenceEquals(a, b) || (a.Id > 0 && b.Id > 0 && a.Id == b.Id);

        private static bool SameRegion(Region a, Region b) =>
            ReferenceEquals(a, b) || (a.Id > 0 && b.Id > 0 && a.Id == b.Id);
    }
}
