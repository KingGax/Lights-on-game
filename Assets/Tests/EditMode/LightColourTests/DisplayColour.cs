using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
public class DisplayColour {

    [Test]
    public void Black() {
        Assert.NotNull(LightColour.Black.DisplayColour());
    }

    [Test]
    public void Red() {
        Assert.NotNull(LightColour.Red.DisplayColour());
    }

    [Test]
    public void Green() {
        Assert.NotNull(LightColour.Green.DisplayColour());
    }

    [Test]
    public void Blue() {
        Assert.NotNull(LightColour.Blue.DisplayColour());
    }

    [Test]
    public void Yellow() {
        Assert.NotNull(LightColour.Yellow.DisplayColour());
    }

    [Test]
    public void Magenta() {
        Assert.NotNull(LightColour.Magenta.DisplayColour());
    }

    [Test]
    public void Cyan() {
        Assert.NotNull(LightColour.Cyan.DisplayColour());
    }

    [Test]
    public void White() {
        Assert.NotNull(LightColour.White.DisplayColour());
    }
}}