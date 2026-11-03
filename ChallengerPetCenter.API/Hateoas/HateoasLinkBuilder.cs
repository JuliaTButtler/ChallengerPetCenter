using ChallengerPetCenter.DTOs.Common;

namespace ChallengerPetCenter.Hateoas
{
    public static class HateoasLinkBuilder
    {
        public static IReadOnlyList<LinkDto> ForPagedCollection(
            string basePath,
            int page,
            int pageSize,
            long total,
            IDictionary<string, string?>? extraQuery = null)
        {
            var links = new List<LinkDto>
            {
                new()
                {
                    Rel = "self",
                    Href = BuildHref(basePath, page, pageSize, extraQuery),
                    Method = "GET"
                }
            };

            var totalPages = pageSize <= 0 ? 0 : (int)Math.Ceiling(total / (double)pageSize);

            if (page > 1)
            {
                links.Add(new LinkDto
                {
                    Rel = "prev",
                    Href = BuildHref(basePath, page - 1, pageSize, extraQuery),
                    Method = "GET"
                });
            }

            if (page < totalPages)
            {
                links.Add(new LinkDto
                {
                    Rel = "next",
                    Href = BuildHref(basePath, page + 1, pageSize, extraQuery),
                    Method = "GET"
                });
            }

            links.Add(new LinkDto
            {
                Rel = "create",
                Href = basePath,
                Method = "POST"
            });

            return links;
        }

        public static IReadOnlyList<LinkDto> ForResource(string basePath, int id)
        {
            var self = $"{basePath}/{id}";
            return new List<LinkDto>
            {
                new() { Rel = "self", Href = self, Method = "GET" },
                new() { Rel = "update", Href = self, Method = "PUT" },
                new() { Rel = "delete", Href = self, Method = "DELETE" }
            };
        }

        private static string BuildHref(
            string basePath,
            int page,
            int pageSize,
            IDictionary<string, string?>? extraQuery)
        {
            var query = new List<string>
            {
                $"page={page}",
                $"pageSize={pageSize}"
            };

            if (extraQuery is not null)
            {
                foreach (var (key, value) in extraQuery)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        query.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
                    }
                }
            }

            return $"{basePath}?{string.Join("&", query)}";
        }
    }
}
