using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LightsOn.LightingSystem;

namespace LightColourTests {
namespace Subtract {
public class Cyan {

    [Test]
    public void SubtractBlack() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Cyan.Subtract(LightColour.Black)
        );
    }

    [Test]
    public void SubtractRed() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Cyan.Subtract(LightColour.Red)
        );
    }

    [Test]
    public void SubtractGreen() {
        Assert.AreEqual(
            LightColour.Blue,
            LightColour.Cyan.Subtract(LightColour.Green)
        );
    }

    [Test]
    public void SubtractBlue() {
        Assert.AreEqual(
            LightColour.Green,
            LightColour.Cyan.Subtract(LightColour.Blue)
        );
    }

    [Test]
    public void SubtractCyan() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Cyan.Subtract(LightColour.Cyan)
        );
    }

    [Test]
    public void SubtractYellow() {
        Assert.AreEqual(
            LightColour.Blue,
            LightColour.Cyan.Subtract(LightColour.Yellow)
        );
    }

    [Test]
    public void SubtractMagenta() {
        Assert.AreEqual(
            LightColour.Green,
            LightColour.Cyan.Subtract(LightColour.Magenta)
        );
    }

    [Test]
    public void SubtractWhite() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Cyan.Subtract(LightColour.White)
        );
    }
}}}