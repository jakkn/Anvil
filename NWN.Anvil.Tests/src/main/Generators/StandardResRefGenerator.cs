using System.Collections.Generic;
using System.IO;
using Anvil.Services;
using NLog;

namespace Anvil.Tests.Generators
{
  /// <summary>
  /// Generates the StandardResRef.cs constants file from the standard palette.
  /// </summary>
  // Bind this class as a service or create a new instance to generate.
  // [ServiceBinding(typeof(StandardResRefGenerator))]
  // [ServiceBindingOptions(BindingPriority = BindingPriority.Highest)]
  internal class StandardResRefGenerator
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    private const string CreaturePaletteResRef = "creaturepal";
    private const string DoorPaletteResRef = "doorpal";
    private const string EncounterPaletteResRef = "encounterpal";
    private const string ItemPaletteResRef = "itempal";
    private const string PlaceablePaletteResRef = "placeablepal";
    private const string SoundPaletteResRef = "soundpal";
    private const string StorePaletteResRef = "storepal";
    private const string TriggerPaletteResRef = "triggerpal";
    private const string WaypointPaletteResRef = "waypointpal";

    private readonly StreamWriter constantWriter;

    public StandardResRefGenerator(InjectionService injectionService, PluginStorageService storageService)
    {
      constantWriter = new StreamWriter(Path.Combine(storageService.GetPluginStoragePath(typeof(StandardResRefGenerator).Assembly), "StandardResRef.cs"));
      WriteConstants(injectionService);
      constantWriter.Dispose();

      Log.Info("Generation complete.");
    }

    private void WriteConstants(InjectionService injectionService)
    {
      constantWriter.WriteLine("// <auto-generated />");
      constantWriter.WriteLine("namespace Anvil.Tests.Resources");
      constantWriter.WriteLine("{");
      constantWriter.WriteLine("  internal static class StandardResRef");
      constantWriter.WriteLine("  {");
      WriteBlueprintConstants("Creature", injectionService.Inject(new Palette(CreaturePaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Door", injectionService.Inject(new Palette(DoorPaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Encounter", injectionService.Inject(new Palette(EncounterPaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Item", injectionService.Inject(new Palette(ItemPaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Placeable", injectionService.Inject(new Palette(PlaceablePaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Sound", injectionService.Inject(new Palette(SoundPaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Store", injectionService.Inject(new Palette(StorePaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Trigger", injectionService.Inject(new Palette(TriggerPaletteResRef)).GetBlueprints());
      constantWriter.WriteLine();
      WriteBlueprintConstants("Waypoint", injectionService.Inject(new Palette(WaypointPaletteResRef)).GetBlueprints());
      constantWriter.WriteLine("  }");
      constantWriter.WriteLine("}");
    }

    private void WriteBlueprintConstants(string name, List<PaletteEntry> blueprints)
    {
      Log.Info($"Generating {name} constants.");

      constantWriter.WriteLine($"    public static class {name}");
      constantWriter.WriteLine("    {");
      foreach (PaletteEntry blueprint in blueprints)
      {
        constantWriter.WriteLine($"      ///{blueprint.Name}");
        constantWriter.WriteLine($"      public const string {blueprint.ResRef} = \"{blueprint.ResRef}\";");
      }

      constantWriter.WriteLine("    }");
    }
  }
}
