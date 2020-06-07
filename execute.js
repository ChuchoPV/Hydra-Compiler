#!/usr/bin/env node

/*
  Hydra WASM execution script.
  Copyright (C) 2020 Ariel Ortiz, ITESM CEM
  Modified by: Gerardo Galván, Jesús Perea, Jorge López.

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

const fs = require('fs');
const hydralib = require('./hydralib.js');
const globals = require("./globals.js");

async function main() {
  try {
    if (process.argv.length != 3) {
      console.log('Please specify the name of the WASM file to execute.');
      process.exit(1);
    }
    const fileName = process.argv[2];
    const buffer = fs.readFileSync(fileName);
    const module = await WebAssembly.compile(buffer);
    const instance = await WebAssembly.instantiate(module, 
      Object.assign(hydralib, globals));
    instance.exports.main();
  } catch (error) {
    console.log(error.message);
  }
}

main();
