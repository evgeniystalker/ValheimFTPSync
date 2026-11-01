using System.Diagnostics.CodeAnalysis;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ValheimFTPSync.Models
{
    internal class AppVersion : IComparable<AppVersion>, IEqualityComparer<AppVersion>
    {
        public Version Version { get; set; }
        public string? Prereliase { get; set; }
        public string? CommitHash { get; set; }

        public AppVersion(Version version, string? prereliase = null, string? commitHash = null)
        {
            Version = version;
            Prereliase = prereliase;
            CommitHash = commitHash;
        }

        public bool Equals(AppVersion? x, AppVersion? y)
        {
            if (ReferenceEquals(x, y)) return true;
            return x?.Version == y?.Version && x?.Prereliase == y?.Prereliase;
        }

        public int GetHashCode([DisallowNull] AppVersion obj)
        {
            return Version.GetHashCode() + Prereliase?.GetHashCode() ?? 0;
        }

        public int CompareTo(AppVersion? other)
        {
            if (other == null) return 1;
            int result = this.Version.CompareTo(other.Version);
            if (result != 0)
                return result;
            return GetStage(Prereliase).CompareTo(GetStage(other.Prereliase));
        }

        public static bool operator <(AppVersion left, AppVersion right)
        {
            if (left is null) return right is not null;
            return left.CompareTo(right) < 0;
        }
        public static bool operator <=(AppVersion left, AppVersion right)
        {
            if (left is null) return true;
            return left.CompareTo(right) <= 0;
        }
        public static bool operator >(AppVersion left, AppVersion right)
        {
            if (left is null) return false;
            return left.CompareTo(right) > 0;
        }
        public static bool operator >=(AppVersion left, AppVersion right)
        {
            if (left is null) return right is null;
            return left.CompareTo(right) >= 0;
        }

        private static int GetStage(string? label) => label?.ToLowerInvariant() switch
        {
            null or "" => 0, // Стабильная версия выше предварительной.
            "alpha" => -1,
            "beta" => -2,
            "rc" => -3,
            _ => throw new ArgumentException($"Неизвестная стадия версии: {label}")
        };

        public static bool TryParse(string? version, [NotNullWhen(true)] out AppVersion? appVersion)
        {
            appVersion = null;
            if (string.IsNullOrWhiteSpace(version)) return false;
            var hashParts = version.Split('+', 2);
            var hash = hashParts.Length > 1 ? hashParts[1] : null;

            var preReleaseParts = hashParts[0].TrimStart('v', 'V').Split('-', 2);
            var tagNumber = preReleaseParts[0];
            var prereleaseLabel = preReleaseParts.Length > 1 ? preReleaseParts[1] : null;
            if (!Version.TryParse(tagNumber, out var numberVersion)) return false;

            appVersion = new AppVersion(numberVersion, prereleaseLabel, hash);
            return true;
        }
    }
}
