using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;
using Newtonsoft.Json;

namespace VolApp.Controllers
{
    public class CarteController : Controller
    {
        private readonly IConfiguration _config;

        public CarteController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetVolsGeoJson()
        {
            var geojson = string.Empty;
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using var conn = new NpgsqlConnection(connectionString);
            conn.Open();

            var cmd = new NpgsqlCommand(@"
                SELECT jsonb_build_object(
                    'type', 'FeatureCollection',
                    'features', jsonb_agg(
                        jsonb_build_object(
                            'type', 'Feature',
                            'geometry', ST_AsGeoJSON(geom)::jsonb,
                            'properties', to_jsonb(v) - 'geom'
                        )
                    )
                )
                FROM vols_lignes v;
            ", conn);

            var result = cmd.ExecuteScalar();
            geojson = result?.ToString() ?? "{}";

            return Content(geojson, "application/json");
        }
    }
}
