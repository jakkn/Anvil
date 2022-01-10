using System.Collections.Generic;
using Anvil.API;
using Anvil.Tests.Resources;
using NUnit.Framework;

namespace Anvil.Tests.API
{
  [TestFixture(Category = "API.Object")]
  public sealed class NwPlaceableTests
  {
    private readonly List<NwGameObject> createdTestObjects = new List<NwGameObject>();

    [Test(Description = "Creating a placeable with a valid ResRef creates a valid placeable.")]
    [TestCase(StandardResRef.Placeable.plc_arrowcorpse)]
    [TestCase(StandardResRef.Placeable.plc_rubble)]
    [TestCase(StandardResRef.Placeable.x3_plc_balc012)]
    [TestCase(StandardResRef.Placeable.x3_plc_banner1)]
    [TestCase(StandardResRef.Placeable.x3_plc_chim001)]
    [TestCase(StandardResRef.Placeable.x3_plc_flagbane2)]
    [TestCase(StandardResRef.Placeable.plc_armoire)]
    [TestCase(StandardResRef.Placeable.plc_barrel)]
    [TestCase(StandardResRef.Placeable.nw_plc_chestburd)]
    [TestCase(StandardResRef.Placeable.plc_corpse1)]
    [TestCase(StandardResRef.Placeable.x0_golempartsbon)]
    public void CreatePlaceableIsCreated(string placeableResRef)
    {
      Location startLocation = NwModule.Instance.StartingLocation;
      NwPlaceable placeable = NwPlaceable.Create(placeableResRef, startLocation);

      Assert.IsNotNull(placeable, $"Placeable {placeableResRef} was null after creation.");
      Assert.IsTrue(placeable.IsValid, $"Placeable {placeableResRef} was invalid after creation.");

      createdTestObjects.Add(placeable);
    }

    [Test(Description = "Cloning a placeable with copyLocalState = true copies expected local state information.")]
    [TestCase(StandardResRef.Placeable.plc_arrowcorpse)]
    [TestCase(StandardResRef.Placeable.plc_rubble)]
    [TestCase(StandardResRef.Placeable.x3_plc_balc012)]
    [TestCase(StandardResRef.Placeable.x3_plc_banner1)]
    [TestCase(StandardResRef.Placeable.x3_plc_chim001)]
    [TestCase(StandardResRef.Placeable.x3_plc_flagbane2)]
    [TestCase(StandardResRef.Placeable.plc_armoire)]
    [TestCase(StandardResRef.Placeable.plc_barrel)]
    [TestCase(StandardResRef.Placeable.nw_plc_chestburd)]
    [TestCase(StandardResRef.Placeable.plc_corpse1)]
    [TestCase(StandardResRef.Placeable.x0_golempartsbon)]
    public void ClonePlaceableWithLocalStateIsCopied(string placeableResRef)
    {
      Location startLocation = NwModule.Instance.StartingLocation;
      NwPlaceable placeable = NwPlaceable.Create(placeableResRef, startLocation);

      Assert.IsNotNull(placeable, $"Placeable {placeableResRef} was null after creation.");
      Assert.IsTrue(placeable.IsValid, $"Placeable {placeableResRef} was invalid after creation.");

      createdTestObjects.Add(placeable);

      LocalVariableInt testVar = placeable.GetObjectVariable<LocalVariableInt>("test");
      testVar.Value = 9999;

      NwPlaceable clone = placeable.Clone(startLocation);

      Assert.IsNotNull(clone, $"Placeable {placeableResRef} was null after clone.");
      Assert.IsTrue(clone.IsValid, $"Placeable {placeableResRef} was invalid after clone.");

      createdTestObjects.Add(clone);

      LocalVariableInt cloneTestVar = clone.GetObjectVariable<LocalVariableInt>("test");

      Assert.IsTrue(cloneTestVar.HasValue, "Local variable did not exist on the clone with copyLocalState = true.");
      Assert.AreEqual(cloneTestVar.Value, testVar.Value, "Local variable on the cloned placeable did not match the value of the original placeable.");
    }

    [Test(Description = "Cloning a placeable with copyLocalState = false does not copy local state information.")]
    [TestCase(StandardResRef.Placeable.plc_arrowcorpse)]
    [TestCase(StandardResRef.Placeable.plc_rubble)]
    [TestCase(StandardResRef.Placeable.x3_plc_balc012)]
    [TestCase(StandardResRef.Placeable.x3_plc_banner1)]
    [TestCase(StandardResRef.Placeable.x3_plc_chim001)]
    [TestCase(StandardResRef.Placeable.x3_plc_flagbane2)]
    [TestCase(StandardResRef.Placeable.plc_armoire)]
    [TestCase(StandardResRef.Placeable.plc_barrel)]
    [TestCase(StandardResRef.Placeable.nw_plc_chestburd)]
    [TestCase(StandardResRef.Placeable.plc_corpse1)]
    [TestCase(StandardResRef.Placeable.x0_golempartsbon)]
    public void ClonePlaceableNoLocalStateIsNotCopied(string placeableResRef)
    {
      Location startLocation = NwModule.Instance.StartingLocation;
      NwPlaceable placeable = NwPlaceable.Create(placeableResRef, startLocation);

      Assert.IsNotNull(placeable, $"Placeable {placeableResRef} was null after creation.");
      Assert.IsTrue(placeable.IsValid, $"Placeable {placeableResRef} was invalid after creation.");

      createdTestObjects.Add(placeable);

      LocalVariableInt testVar = placeable.GetObjectVariable<LocalVariableInt>("test");
      testVar.Value = 9999;

      NwPlaceable clone = placeable.Clone(startLocation, null, false);

      Assert.IsNotNull(clone, $"Placeable {placeableResRef} was null after clone.");
      Assert.IsTrue(clone.IsValid, $"Placeable {placeableResRef} was invalid after clone.");

      createdTestObjects.Add(clone);

      LocalVariableInt cloneTestVar = clone.GetObjectVariable<LocalVariableInt>("test");

      Assert.IsFalse(cloneTestVar.HasValue, "Local variable exists on the clone with copyLocalState = false.");
      Assert.AreNotEqual(cloneTestVar.Value, testVar.Value, "Local variable on the cloned placeable matches the value of the original placeable.");
    }

    [Test(Description = "Cloning a placeable with a custom tag defined causes the new placeable to gain the new tag.")]
    [TestCase(StandardResRef.Placeable.plc_arrowcorpse)]
    [TestCase(StandardResRef.Placeable.plc_rubble)]
    [TestCase(StandardResRef.Placeable.x3_plc_balc012)]
    [TestCase(StandardResRef.Placeable.x3_plc_banner1)]
    [TestCase(StandardResRef.Placeable.x3_plc_chim001)]
    [TestCase(StandardResRef.Placeable.x3_plc_flagbane2)]
    [TestCase(StandardResRef.Placeable.plc_armoire)]
    [TestCase(StandardResRef.Placeable.plc_barrel)]
    [TestCase(StandardResRef.Placeable.nw_plc_chestburd)]
    [TestCase(StandardResRef.Placeable.plc_corpse1)]
    [TestCase(StandardResRef.Placeable.x0_golempartsbon)]
    public void ClonePlaceableCustomTagIsApplied(string placeableResRef)
    {
      Location startLocation = NwModule.Instance.StartingLocation;
      NwPlaceable placeable = NwPlaceable.Create(placeableResRef, startLocation);

      Assert.IsNotNull(placeable, $"Placeable {placeableResRef} was null after creation.");
      Assert.IsTrue(placeable.IsValid, $"Placeable {placeableResRef} was invalid after creation.");

      createdTestObjects.Add(placeable);

      string expectedNewTag = "expectedNewTag";
      NwPlaceable clone = placeable.Clone(startLocation, expectedNewTag, false);

      Assert.IsNotNull(clone, $"Placeable {placeableResRef} was null after clone.");
      Assert.IsTrue(clone.IsValid, $"Placeable {placeableResRef} was invalid after clone.");

      createdTestObjects.Add(clone);

      Assert.AreEqual(expectedNewTag, clone.Tag, "Tag defined in clone method was not applied to the cloned placeable.");
    }

    [Test(Description = "Cloning a placeable with no tag defined uses the original placeable's tag instead.")]
    [TestCase(StandardResRef.Placeable.plc_arrowcorpse)]
    [TestCase(StandardResRef.Placeable.plc_rubble)]
    [TestCase(StandardResRef.Placeable.x3_plc_balc012)]
    [TestCase(StandardResRef.Placeable.x3_plc_banner1)]
    [TestCase(StandardResRef.Placeable.x3_plc_chim001)]
    [TestCase(StandardResRef.Placeable.x3_plc_flagbane2)]
    [TestCase(StandardResRef.Placeable.plc_armoire)]
    [TestCase(StandardResRef.Placeable.plc_barrel)]
    [TestCase(StandardResRef.Placeable.nw_plc_chestburd)]
    [TestCase(StandardResRef.Placeable.plc_corpse1)]
    [TestCase(StandardResRef.Placeable.x0_golempartsbon)]
    public void ClonePlaceableWithoutTagOriginalTagIsCopied(string placeableResRef)
    {
      Location startLocation = NwModule.Instance.StartingLocation;
      NwPlaceable placeable = NwPlaceable.Create(placeableResRef, startLocation);
      placeable.Tag = "expectedNewTag";

      Assert.IsNotNull(placeable, $"Placeable {placeableResRef} was null after creation.");
      Assert.IsTrue(placeable.IsValid, $"Placeable {placeableResRef} was invalid after creation.");

      createdTestObjects.Add(placeable);

      NwPlaceable clone = placeable.Clone(startLocation, null, false);

      Assert.IsNotNull(clone, $"Placeable {placeableResRef} was null after clone.");
      Assert.IsTrue(clone.IsValid, $"Placeable {placeableResRef} was invalid after clone.");

      createdTestObjects.Add(clone);

      Assert.AreEqual(placeable.Tag, clone.Tag, "Cloned placeable's tag did not match the original placeable's.");
    }

    [TearDown]
    public void CleanupTestObject()
    {
      foreach (NwGameObject testObject in createdTestObjects)
      {
        testObject.PlotFlag = false;
        testObject.Destroy();
      }

      createdTestObjects.Clear();
    }
  }
}
