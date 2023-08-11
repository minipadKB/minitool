<div align="center">

# minitool

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Discord](https://img.shields.io/discord/1056311828344483840?label=discord&color=7289da)](https://discord.gg/minipad)
[![Downloads](https://img.shields.io/github/downloads/minipadkb/minitool/total)](https://github.com/minipadKB/minitool/releases/latest)
[![Latest Release](https://img.shields.io/github/v/release/minipadkb/minitool?color=dd00dd)](https://github.com/minipadKB/minitool/releases/latest)

The command-line tool for the minipad, an RP2040-based 2-3-key keypad for the rhythm game osu!.

This tool is designed specifically to work with our open-source firmware,</br>
which can be found in our GitHub repository [here](https://github.com/minipadkb/minipad-firmware).

**Note: Currently, only Windows is supported.**

[Usage](#usage)

</div>

<div align="center">
<i>Made with ❤️ by Project Minipad</i>
</div>

# Usage

This command-line utility can be used by opening the Windows Terminal or command prompt (*not powershell*) in the folder of the minitool.exe executable. Below you can find a list of commands.</br>
**Note:** 'port' is always referring to the port number, excluding the `COM` prefix. (e.g. `4` instead of `COM4`)

General syntax: `minitool.exe <sub-command> <parameters...>`

Command: `devices`</br>
Description: Lists all connected minipad devices.</br>
Example: `minitool.exe devices`

Command: `info <port>`</br>
Description: Outputs the configuration of the minipad device.</br>
Example: `minitool.exe info 7`

Command: `boot <port>`</br>
Description: Sets the minipad device into bootloader mode.</br>
Example: `minitool.exe boot 4`</br>

Command: `send <port> "<command>"`</br>
Description: Sends the specified command to the minipad device.</br>
Example: `minitool.exe send 9 "hkey.rt 1"`</br>

Command: `calibrate <port>`</br>
Description: Helps you calibrating the minipad by pressing the keys down.</br>
Example: `minitool.exe calibrate 3`</br>

Command: `visualize <port>`</br>
Description: Displays a commandline-based visualizer for the Hall Effect keys.</br>
Example: `minitool.exe visualize 14`</br>

You can also find a list of these commands with `minitool.exe help`.

**A list of commands for the minipad firmware can be found [here](https://github.com/minipadKB/minipad-firmware/tree/master#minipad-serial-protocol-msp-).**
