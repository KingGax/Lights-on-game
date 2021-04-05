using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
namespace Subtract {
public class Black {

    [Test]
    public void SubtractBlack() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.Black)
        );
    }

    [Test]
    public void SubtractRed() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.Red)
        );
    }

    [Test]
    public void SubtractGreen() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.Green)
        );
    }

    [Test]
    public void SubtractBlue() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.Blue)
        );
    }

    [Test]
    public void SubtractCyan() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.Cyan)
        );
    }

    [Test]
    public void SubtractYellow() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.Yellow)
        );
    }

    [Test]
    public void SubtractMagenta() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.Magenta)
        );
    }

    [Test]
    public void SubtractWhite() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.Black.Subtract(LightColour.White)
        );
    }
} } }