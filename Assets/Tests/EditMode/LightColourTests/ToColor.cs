using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
public class ToColor {

    [Test]
    public void Black() {
        Assert.NotNull(LightColour.Black.ToColor());
    }

    [Test]
    public void Red() {
        Assert.NotNull(LightColour.Red.ToColor());
    }

    [Test]
    public void Green() {
        Assert.NotNull(LightColour.Green.ToColor());
    }

    [Test]
    public void Blue() {
        Assert.NotNull(LightColour.Blue.ToColor());
    }

    [Test]
    public void Yellow() {
        Assert.NotNull(LightColour.Yellow.ToColor());
    }

    [Test]
    public void Magenta() {
        Assert.NotNull(LightColour.Magenta.ToColor());
    }

    [Test]
    public void Cyan() {
        Assert.NotNull(LightColour.Cyan.ToColor());
    }

    [Test]
    public void White() {
        Assert.NotNull(LightColour.White.ToColor());
    }
}}