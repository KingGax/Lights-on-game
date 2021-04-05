using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace LightColourTests {
namespace MergeWith {
public class Cyan {

    [Test]
    public void MergeWithBlack() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Cyan.MergeWith(LightColour.Black)
        );
    }

    [Test]
    public void MergeWithRed() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Cyan.MergeWith(LightColour.Red)
        );
    }

    [Test]
    public void MergeWithGreen() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Cyan.MergeWith(LightColour.Green)
        );
    }

    [Test]
    public void MergeWithBlue() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Cyan.MergeWith(LightColour.Blue)
        );
    }

    [Test]
    public void MergeWithCyan() {
        Assert.AreEqual(
            LightColour.Cyan,
            LightColour.Cyan.MergeWith(LightColour.Cyan)
        );
    }

    [Test]
    public void MergeWithYellow() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Cyan.MergeWith(LightColour.Yellow)
        );
    }

    [Test]
    public void MergeWithMagenta() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Cyan.MergeWith(LightColour.Magenta)
        );
    }

    [Test]
    public void MergeWithWhite() {
        Assert.AreEqual(
            LightColour.White,
            LightColour.Cyan.MergeWith(LightColour.White)
        );
    }
} } }