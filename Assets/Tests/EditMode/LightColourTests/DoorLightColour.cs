using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LightsOn.LightingSystem;

namespace LightColourTests {
public class DoorLightColour {

    [Test]
    public void Black() {
        Assert.NotNull(LightColour.Black.DoorLightColour());
    }

    [Test]
    public void Red() {
        Assert.NotNull(LightColour.Red.DoorLightColour());
    }

    [Test]
    public void Green() {
        Assert.NotNull(LightColour.Green.DoorLightColour());
    }

    [Test]
    public void Blue() {
        Assert.NotNull(LightColour.Blue.DoorLightColour());
    }

    [Test]
    public void Yellow() {
        Assert.NotNull(LightColour.Yellow.DoorLightColour());
    }

    [Test]
    public void Magenta() {
        Assert.NotNull(LightColour.Magenta.DoorLightColour());
    }

    [Test]
    public void Cyan() {
        Assert.NotNull(LightColour.Cyan.DoorLightColour());
    }

    [Test]
    public void White() {
        Assert.NotNull(LightColour.White.DoorLightColour());
    }
}}