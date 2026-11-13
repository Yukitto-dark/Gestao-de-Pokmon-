-- 1. Total de habilidades no catálogo.
SELECT COUNT(*) AS TotalHabilidades FROM Abilities;

-- 2. Total de relacionamentos entre formas e habilidades.
SELECT COUNT(*) AS TotalRelacionamentos FROM FormAbilities;

-- 3. Pokémon com pelo menos uma habilidade na forma Normal.
SELECT COUNT(DISTINCT f.PokemonId) AS PokemonComHabilidades
FROM FormAbilities fa
JOIN PokemonForms f ON f.Id = fa.PokemonFormId
WHERE lower(f.FormName) = 'normal';

-- 4. Relação completa: Pokémon, habilidade e slot.
SELECT
    p.NationalDexNumber,
    p.Name AS Pokemon,
    a.Name AS Habilidade,
    CASE fa.Slot
        WHEN 0 THEN 'Slot1'
        WHEN 1 THEN 'Slot2'
        WHEN 2 THEN 'Hidden'
        ELSE 'Desconhecido'
    END AS Slot
FROM FormAbilities fa
JOIN PokemonForms f ON f.Id = fa.PokemonFormId
JOIN Pokemons p ON p.Id = f.PokemonId
JOIN Abilities a ON a.Id = fa.AbilityId
WHERE lower(f.FormName) = 'normal'
ORDER BY p.NationalDexNumber, fa.Slot;

-- 5. Pokémon do cadastro #001–#386 que ainda não possuem habilidade normal.
SELECT p.NationalDexNumber, p.Name
FROM Pokemons p
WHERE p.NationalDexNumber BETWEEN 1 AND 386
  AND NOT EXISTS (
      SELECT 1
      FROM PokemonForms f
      JOIN FormAbilities fa ON fa.PokemonFormId = f.Id
      WHERE f.PokemonId = p.Id
        AND lower(f.FormName) = 'normal'
  )
ORDER BY p.NationalDexNumber;

-- 6. Verificar integridade referencial (resultado esperado: vazio).
PRAGMA foreign_key_check;
