using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
namespace Subtract {
public class Yellow {

    [Test]
    public void SubtractBlack() {
        Assert.AreEqual(
            LightColour.Yellow,
            LightColour.Yellow.Subtract(LightColour.Black)
        );
    }

    [Test]
    public void SubtractRed() {
        Assert.AreEqual(
            LightColour.Green,
            LightColour.Yellow.Subtract(LightColour.Red)
        );
    }

    [Test]
    public void SubtractGreen() {
        Assert.AreEqual(
            LightColour.Red,
            LightColour.Yellow.Subtract(LightColour.Green)
        );
    }

    [Test]
    public void SubtractBlue() {
        Assert.AreEqual(
            LightColour.Yellow,
            LightColour.Yellow.Subtract(LightColour.Blue)
        );
    }

    [Test]
    public void SubtractCyan() {
        Assert.AreEqual(
            LightColour.Red,
            LightColour.Yellow.Subtract(LightColour.Cyan)
        );
    }

    [Test]
    public void SubtractYellow() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Yellow.Subtract(LightColour.Yellow)
        );
    }

    [Test]
    public void SubtractMagenta() {
        Assert.AreEqual(
            LightColour.Green,
            LightColour.Yellow.Subtract(LightColour.Magenta)
        );
    }

    [Test]
    public void SubtractWhite() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Yellow.Subtract(LightColour.White)
        );
    }
} } }