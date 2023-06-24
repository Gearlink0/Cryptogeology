using Genkit;
using Qud.API;
using XRL.World;
using XRL.World.Parts;
using XRL.World;

// namespace YourMod.YourNamespace
// namespace CRYPTOGEOLOGY
namespace XRL.World.WorldBuilders
{
  [JoppaWorldBuilderExtension]
  public class CRYPTOGEOLOGY_DigSiteBuilderExtension : IJoppaWorldBuilderExtension
  {
    private string World = "JoppaWorld";
    public string secretId = "$CRYPTOGEOLOGY_digsite";

    public override void OnAfterBuild(JoppaWorldBuilder builder)
    {
      // The game calls this method before JoppaWorld generation takes place.
      // JoppaWorld generation includes the creation of lairs, historic ruins, villages, and more.
      MetricsManager.LogInfo("CRYPTOGEOLOGY_DigSiteBuilderExtension running");
			Location2D c2 = builder.mutableMap.popMutableLocationInArea(3, 3, 12, 24);
      string zone = Zone.XYToID(this.World, c2.x, c2.y, 10);
      Cell cell = The.ZoneManager.GetZone(this.World).GetCell(c2.x / 3, c2.y / 3);
      The.ZoneManager.ClearZoneBuilders(zone);
      The.ZoneManager.AddZonePostBuilder(zone, new ZoneBuilderBlueprint("ClearAll"));
      The.ZoneManager.AddZonePostBuilder(zone, new ZoneBuilderBlueprint("MapBuilder", "FileName", (object) "DigSite.rpm"));
      The.ZoneManager.AddZonePostBuilder(zone, new ZoneBuilderBlueprint("Music", "Track", (object) "MoghrayiRemembrance"));

      GeneratedLocationInfo generatedLocationInfo = new GeneratedLocationInfo();
      generatedLocationInfo.name = "The Dig Site";
      generatedLocationInfo.targetZone = zone;
      generatedLocationInfo.zoneLocation = Location2D.get(c2.x, c2.y);
      generatedLocationInfo.secretID = (string) null;
      builder.worldInfo.villages.Add(generatedLocationInfo);
      builder.mutableMap.SetMutable(generatedLocationInfo.zoneLocation, 0);
      builder.game.SetStringGameState("CRYPTOGEOLOGY_DigSiteZoneID", Zone.XYToID(this.World, c2.x, c2.y, 10));
      // Secretzone, name, adj, category, secret ID
      builder.AddSecret(
        Zone.XYToID(this.World, c2.x, c2.y, 10),
        "The Dig Site",
        new string[1] {
	        "oddity"
	      },
        "Oddities",
        this.secretId
      );
    }
	}
}
