using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using LightsOn.LightingSystem;

namespace LightColourTests {
namespace MergeWith {
public class Red {

    [Test]
    public void MergeWithBlack() {
        Assert.AreEqual(
            LightColour.Red,
            LightColour.Red.MergeWith(LightColour.Black)
        );
    }

    [Test]
    public void MergeWithRed() {
        Assert.AreEqual(
            LightColour.Red,
            LightColour.Red.MergeWith(LightColour.Red)
        );
    }

    [Test]
    public void MergeWithGreen() {
        Assert.AreEqual(
            LightColour.Yellow,
            LightColour.Red.MergeWith(LightColour.Green)
        );
    }

    [Test]
    public void MergeWithBlue() {
        Assert.AreEqual(
            LightColour.Magenta,
            LightColour.Red.MergeWith(LightColour.Blue)
        );
    }

    [Test]
    public void MergeWithCyan() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Red.MergeWith(LightColour.Cyan)
        );
    }

    [Test]
    public void MergeWithYellow() {
        Assert.AreEqual(
            LightColour.Yellow,
            LightColour.Red.MergeWith(LightColour.Yellow)
        );
    }

    [Test]
    public void MergeWithMagenta() {
        Assert.AreEqual(
            LightColour.Magenta,
            LightColour.Red.MergeWith(LightColour.Magenta)
        );
    }

    [Test]
    public void MergeWithWhite() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Red.MergeWith(LightColour.White)
        );
    }
}}}