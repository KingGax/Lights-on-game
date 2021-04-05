using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
namespace Subtract {
public class White {

    [Test]
    public void SubtractBlack() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.White.Subtract(LightColour.Black)
        );
    }

    [Test]
    public void SubtractRed() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.White.Subtract(LightColour.Red)
        );
    }

    [Test]
    public void SubtractGreen() {
        Assert.AreEqual(
            LightColour.Magenta,
            LightColour.White.Subtract(LightColour.Green)
        );
    }

    [Test]
    public void SubtractBlue() {
        Assert.AreEqual(
            LightColour.Yellow,
            LightColour.White.Subtract(LightColour.Blue)
        );
    }

    [Test]
    public void SubtractCyan() {
        Assert.AreEqual(
            LightColour.Red,
            LightColour.White.Subtract(LightColour.Cyan)
        );
    }

    [Test]
    public void SubtractYellow() {
        Assert.AreEqual(
            LightColour.Blue,
            LightColour.White.Subtract(LightColour.Yellow)
        );
    }

    [Test]
    public void SubtractMagenta() {
        Assert.AreEqual(
            LightColour.Green,
            LightColour.White.Subtract(LightColour.Magenta)
        );
    }

    [Test]
    public void SubtractWhite() {
        Assert.AreEqual(
            LightColour.Black,
            LightColour.White.Subtract(LightColour.White)
        );
    }
} } }