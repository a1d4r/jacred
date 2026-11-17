using JacRed.Infrastructure.Indexers;
using JacRed.Infrastructure.Utils;
using JacRed.Models.Details;
using Xunit;

namespace JacRed.Tests.Indexers;

// Names and titles of real releases, as JacRed stores them.
public class ReleaseTitleFormsTests
{
    [Theory]
    // Episodes, for any search
    [InlineData("110из10", 0, true)]
    [InlineData("12сериииз2", 0, true)]
    [InlineData("2из2расширеннаяверсия", 0, true)]
    // A season only for that season
    [InlineData("2", 2, true)]
    [InlineData("2ndseason", 2, true)]
    [InlineData("season2", 2, true)]
    [InlineData("tv2", 2, true)]
    [InlineData("тв2", 2, true)]
    [InlineData("сезон2", 2, true)]
    [InlineData("второйсезон", 2, true)]
    [InlineData("2восстаньтеизтени", 2, true)]
    [InlineData("2", 1, false)]
    [InlineData("2", 0, false)]
    [InlineData("20", 2, false)]
    [InlineData("2023", 2, false)]
    [InlineData("tv20", 2, false)]
    [InlineData("третийсезон", 2, false)]
    // Letters are subtitles, left to the caller
    [InlineData("arisefromtheshadow", 2, false)]
    public void IsCandidateRest(string rest, int season, bool expected)
    {
        Assert.Equal(expected, ReleaseTitleForms.IsCandidateRest(rest, season));
    }

    [Theory]
    // Episodes in the name
    [InlineData("Холод (1-10 из 10)", "Холод (1-10 из 10) (2026) WEBRip | 4К", "Холод", 0, true)]
    [InlineData("...А зори здесь тихие [1-2 серии из 2]", "...А зори здесь тихие [1-2 серии из 2] (1972) WEB-DL 1080p", "...А зори здесь тихие", 0, true)]
    [InlineData("А зори здесь тихие (2 из 2) (Расширенная версия)", "А зори здесь тихие (2 из 2) (Расширенная версия) (1972) DVDRip", "А зори здесь тихие", 0, true)]
    [InlineData("Место встречи изменить нельзя [01-05 из 05]", "Место встречи изменить нельзя [01-05 из 05] (1979) HDTVRip-AVC", "Место встречи изменить нельзя", 1, true)]
    [InlineData("Холодное сердце (1-2 из 2)", "Холодное сердце (1-2 из 2)", "Холод", 0, false)]
    // A season in the name, for that season, when the release names it
    [InlineData("Монолог фармацевта 2", "Монолог фармацевта 2 / Kusuriya no Hitorigoto 2nd Season / 2025 / [WEBRip 1080p][HEVC][1-24]", "Монолог фармацевта", 2, true)]
    [InlineData("Kusuriya no Hitorigoto 2nd Season", "Монолог фармацевта 2 / Kusuriya no Hitorigoto 2nd Season / 2025 / [WEBRip 1080p][HEVC][1-24]", "Kusuriya no Hitorigoto", 2, true)]
    [InlineData("JUJUTSU KAISEN Season 2", "Магическая битва 2 / JUJUTSU KAISEN Season 2 / 2023 / [WEBRip 1080p][HEVC][1-23]", "Jujutsu Kaisen", 2, true)]
    [InlineData("Dandadan ТВ-2", "Дандадан TV-2 / Dandadan ТВ-2 [12 из 12]", "Dandadan", 2, true)]
    [InlineData("Магическая Битва (Второй Сезон)", "Магическая Битва (Второй Сезон) / Jujutsu Kaisen 2nd Season [02x01-04 из 23] (2023) WEB-DLRip 1080p", "Магическая битва", 2, true)]
    [InlineData("Shingeki no Kyojin Season 3", "Shingeki no Kyojin Season 3 / Вторжение гигантов [ТВ-3, часть 1] (12 из 12) Complete [720p]", "Shingeki no Kyojin", 3, true)]
    [InlineData("Поднятие уровня в одиночку 2: Восстаньте из тени", "Поднятие уровня в одиночку 2: Восстаньте из тени / Solo Leveling 2 [02x01-04 из 13] (2025) WEBRip 1080p", "Поднятие уровня в одиночку", 2, true)]
    [InlineData("Монолог фармацевта 2", "Монолог фармацевта 2 / Kusuriya no Hitorigoto 2nd Season / 2025 / [WEBRip 1080p][HEVC][1-24]", "Монолог фармацевта", 1, false)]
    [InlineData("Монолог фармацевта 2", "Монолог фармацевта 2 / Kusuriya no Hitorigoto 2nd Season / 2025 / [WEBRip 1080p][HEVC][1-24]", "Монолог фармацевта", 0, false)]
    // A film sequel does not name a season
    [InlineData("Дюна 2", "Дюна 2 / Dune: Part Two (2024) BDRip 1080p", "Дюна", 2, false)]
    // A subtitle, for a season search, when the release names that season
    [InlineData("Solo Leveling: Arise from the Shadow", "Solo Leveling: Arise from the Shadow / Ore dake Level Up na Ken: Arise from the Shadow / Поднятие уровня в одиночку [ТВ-2] (13 из 13) Complete [1080p]", "Solo Leveling", 2, true)]
    [InlineData("Solo Leveling: Arise from the Shadow", "Solo Leveling: Arise from the Shadow / Ore dake Level Up na Ken: Arise from the Shadow / Поднятие уровня в одиночку [ТВ-2] (13 из 13) Complete [1080p]", "Solo Leveling", 1, false)]
    [InlineData("Атака титанов: финал - заключительная глава", "Атака титанов: финал - заключительная глава / Attack on Titan: The Final Season Part 3", "Атака титанов", 3, false)]
    [InlineData("Побег: Продолжение", "Побег: Продолжение / Prison Break: Sequel (2017) WEB-DLRip", "Побег", 1, false)]
    // A subtitle behind a first season is a spin-off or a revival
    [InlineData("Побег: Продолжение", "Побег: Продолжение (1 сезон: 1-9 серии из 9) / Prison Break: Sequel / 2017 / WEB-DL (1080p)", "Побег", 1, false)]
    public void Matches_Name(string name, string title, string search, int season, bool expected)
    {
        var t = new TorrentDetails { name = name, originalname = name, title = title, seasons = new HashSet<int>() };

        Assert.Equal(expected, ReleaseTitleForms.Matches(t, StringConvert.SearchName(search), null, season));
    }

    [Theory]
    [InlineData("Холод (1-10 из 10)", "Холод (1-10 из 10)", "Холод", 10)]
    [InlineData("...А зори здесь тихие [1-2 серии из 2]", "...А зори здесь тихие [1-2 серии из 2]", "...А зори здесь тихие", 2)]
    [InlineData("Место встречи изменить нельзя [01-05 из 05]", "Place of Meeting", "Место встречи изменить нельзя", 5)]
    [InlineData("Холод", "Cold (1-6 из 6)", "Cold", 6)]
    [InlineData("Холод (1-2 из 2)", "Холод (1-8 из 8)", "Холод", 8)]
    // Not right behind the title, or no "из N"
    [InlineData("Холодное сердце (1-2 из 2)", "Холодное сердце (1-2 из 2)", "Холод", 0)]
    [InlineData("Холод (2)", "Холод (2)", "Холод", 0)]
    [InlineData("Холод", "Холод", "Холод", 0)]
    public void EpisodesTotal(string name, string originalname, string search, int expected)
    {
        var t = new TorrentDetails { name = name, originalname = originalname };

        Assert.Equal(expected, ReleaseTitleForms.EpisodesTotal(t, StringConvert.SearchName(search), null));
    }

    [Fact]
    public void Matches_StoredSeasonsWinOverTitle()
    {
        var t = new TorrentDetails
        {
            name = "Магическая битва 2",
            originalname = "JUJUTSU KAISEN Season 2",
            title = "Магическая битва 2 / JUJUTSU KAISEN Season 2 [S02] (2023) WEBRip 1080p",
            seasons = new HashSet<int> { 1 }
        };

        Assert.False(ReleaseTitleForms.Matches(t, "магическаябитва", "jujutsukaisen", 2));
        t.seasons = new HashSet<int> { 2 };
        Assert.True(ReleaseTitleForms.Matches(t, "магическаябитва", "jujutsukaisen", 2));
    }

    [Theory]
    [InlineData("Побег из тюрьмы (Сезон 1, серии 1-22(22)) / Prison Break (Пол Шойринг) [2005, США, Экшн, Драма, BDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison Break - сезон 1 - (Пол Шойринг / Paul Scheuring) Nova + Eng [2005, США, драма, боевик, BDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы Сезон 1 (Серия 1-22) / Prison Break Season 1 (Episode 1-22) (Пол Шойринг) [2005, США, BDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison Break / Сезон: 1 / Серии: 1-22 (22) (Пол Шеринг / Paul Scheuring) [2005, США, HDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison Break S01 [2005, США, BDRip]", "Prison Break", true)]
    [InlineData("Закон и порядок (Сезон 1) / Law &amp; Order [1990, США, DVDRip]", "Law & Order", true)]
    // A season range, a part, a colon, dots between words
    [InlineData("Атака титанов: Финал (ТВ-4, часть 2) / Shingeki no Kyojin: The Final Season / Attack on Titan: 4th Season Part 2 [TV] [12 из 12] [2022, WEB-DL] [1080p]", "Attack on Titan", true)]
    [InlineData("Побег из тюрьмы / Prison Break Season 1-5 [2005-2017, США, DVDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison Break S01-S05 [2005-2017, США, DVDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison Break S01E01-22 [2005, США, DVDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison Break - Сезон: 1 [2005, США, DVDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison Break Season 1 Part 2 [2005, США, DVDRip]", "Prison Break", true)]
    [InlineData("Побег из тюрьмы / Prison.Break.S01 [2005, США, DVDRip]", "Prison Break", true)]
    [InlineData("Друзья 9 сезон / Friends 9 season [2003, США, Комедия, DVD5]", "Friends", true)]
    [InlineData("Друзья 10-й сезон / Friends 10-th season (David Crane) [2004, США, Комедийный сериал, DVDRip]", "Friends", true)]
    [InlineData("Атака титанов: Финал (ТВ-4, часть 3) / Shingeki no Kyojin: The Final Season - Kanketsu-hen / Attack on Titan: 4th Season Part 3 The Final Chapters [Special] [2 из 2]", "Attack on Titan", true)]
    // Not a season behind the name: another word, or more after the season
    [InlineData("Вверх тормашками / Ups 1 [2010, Россия, WEBRip]", "Up", false)]
    [InlineData("Вверх тормашками / Up S1 Extra [2010, Россия, WEBRip]", "Up", false)]
    [InlineData("Закон и порядок: Специальный корпус (Сезон 1) / Law & Order: SVU [1999, США]", "Law & Order", false)]
    // The English name only as the first piece, or not at all
    [InlineData("Prison Break (Сезон 1) [2005, США, BDRip]", "Prison Break", false)]
    [InlineData("Одиссея капитана Блада / Captain Blood (Майкл Кертиц) [1935, США, DVDRip]", "The Odyssey", false)]
    public void TitleNamesOriginal(string title, string original, bool expected)
    {
        Assert.Equal(expected, ReleaseTitleForms.TitleNamesOriginal(title, StringConvert.SearchName(original)));
    }

    [Fact]
    public void TitleNamesOriginal_LongRunsOfSpacesAreQuick()
    {
        string title = "Побег из тюрьмы / Prison Break" + new string(' ', 50_000) + "!";

        var sw = System.Diagnostics.Stopwatch.StartNew();
        Assert.False(ReleaseTitleForms.TitleNamesOriginal(title, "prisonbreak1"));
        Assert.True(sw.ElapsedMilliseconds < 500, $"{sw.ElapsedMilliseconds} ms");
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("(")]
    [InlineData(" ( ")]
    [InlineData("x 2 ")]
    public void Matches_LongRunsOfSpacesAreQuick(string unit)
    {
        // A lazy title then optional spaces, a bracket and spaces split a run of spaces cubically many ways:
        // 1000 spaces took seconds.
        string name = "Монолог фармацевта" + string.Concat(Enumerable.Repeat(unit, 1000)) + "!";
        var t = new TorrentDetails { name = name, originalname = name, title = "Монолог фармацевта (2025)", seasons = new HashSet<int>() };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        Assert.False(ReleaseTitleForms.Matches(t, "монологфармацевта", "kusuriyanohitorigoto", 2));
        Assert.True(sw.ElapsedMilliseconds < 500, $"{sw.ElapsedMilliseconds} ms");
    }
}
