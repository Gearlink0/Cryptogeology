using Genkit;
using Qud.API;
using XRL.World;
using XRL.World.Parts;

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
			Location2D location = builder.mutableMap.popMutableLocationInArea(3, 3, 12, 24);
      string zone = Zone.XYToID(this.World, location.X, location.Y, 10);
      Cell cell = The.ZoneManager.GetZone(this.World).GetCell(location.X / 3, location.Y / 3);
      The.ZoneManager.ClearZoneBuilders(zone);
      The.ZoneManager.AddZonePostBuilder(zone, "ClearAll");
      The.ZoneManager.AddZonePostBuilder(zone, "MapBuilder", "FileName", (object) "DigSite.rpm");
      The.ZoneManager.AddZonePostBuilder(zone, "Music", "Track", (object) "MoghrayiRemembrance");

      GeneratedLocationInfo generatedLocationInfo = new GeneratedLocationInfo();
      generatedLocationInfo.name = "The Dig Site";
      generatedLocationInfo.targetZone = zone;
      generatedLocationInfo.zoneLocation = Location2D.Get(location.X, location.Y);
      generatedLocationInfo.secretID = (string) null;
      builder.worldInfo.villages.Add(generatedLocationInfo);
      builder.mutableMap.SetMutable(generatedLocationInfo.zoneLocation, 0);
      builder.game.SetStringGameState("CRYPTOGEOLOGY_DigSiteZoneID", Zone.XYToID(this.World, location.X, location.Y, 10));
      builder.AddSecret(
        secretZone: Zone.XYToID(this.World, location.X, location.Y, 10),
        name: "The Dig Site",
        adj: new string[1] { "oddity" },
        category: "Oddities",
        secretid: this.secretId
      );
    }
	}
}
