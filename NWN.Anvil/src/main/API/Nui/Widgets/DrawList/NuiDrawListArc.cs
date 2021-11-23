using Newtonsoft.Json;

namespace Anvil.API
{
  public sealed class NuiDrawListArc : NuiDrawListItem
  {
    public override NuiDrawListItemType Type
    {
      get => NuiDrawListItemType.Arc;
    }

    [JsonProperty("c")]
    public NuiProperty<NuiVector> Center { get; set; }

    [JsonProperty("radius")]
    public NuiProperty<float> Radius { get; set; }

    [JsonProperty("amin")]
    public NuiProperty<float> AngleMin { get; set; }

    [JsonProperty("amax")]
    public NuiProperty<float> AngleMax { get; set; }

    [JsonConstructor]
    public NuiDrawListArc(NuiProperty<NuiColor> color, NuiProperty<bool> fill, NuiProperty<float> lineThickness, NuiProperty<NuiVector> center, NuiProperty<float> radius,
      NuiProperty<float> angleMin, NuiProperty<float> angleMax) : base(color, fill, lineThickness)
    {
      Center = center;
      Radius = radius;
      AngleMin = angleMin;
      AngleMax = angleMax;
    }
  }
}