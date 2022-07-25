// SPDX-License-Identifier: MIT


/**
 * Deploy all the contracts required for a Toshimon game deployment
 * This includes:
 *   - The Adjudicator
 *   - The ToshimonStateTransition ForceMoveApp
 *   - Every move, item and status condition contract in the respective directories (e.g contracts/Toshimon/moves/*.sol)
 *   
 * All of the deployed moves, items and conditions are also recorded in a json file each which maps their Id to the contract address
 */

var glob = require("glob");
const path = require('path');

module.exports = async ({getNamedAccounts, deployments}) => {
  const {deploy, execute, log, saveDotFile} = deployments;
  const {deployer} = await getNamedAccounts();

  // deploy the library contract ToshimonUtils
  // This needs to be passed to all contracts that make use of 
  // it as a singleton using the libraries field in the deploy options
  const utils = await deploy('ToshimonUtils', {
    from: deployer,
    args: [],
    log: true,
  });

  await deploy('Adjudicator', {
    from: deployer,
    args: [],
    log: true,
  });

  await deploy('ToshimonStateTransition', {
    from: deployer,
    args: [],
    log: true,
  });

  await deploy('TESTNitroUtils', {
    from: deployer,
    args: [],
    log: true,
  });

  // Helper functions

  // From a directory containing .sol files with the naming scheme
  // <XXX>_<ContractName>.sol where XXX is a three digit index with leading zeroes (eg. 003 or 124)
  // grab all the indices and contract names in a list
  function getMatchingArtifacts(filepath) {
    const pattern = /(\d{3})_(\w+).sol/;
    const files = glob.sync(path.join(__dirname, filepath));
    return files.map((fname) => {
      const [_, indexStr, name] = fname.match(pattern);
      return [parseInt(indexStr), name];
    });
  }

  async function deployLibraryUser(contractName, libs) {
    return deploy(contractName, {
      from: deployer,
      args: [],
      log: true,
      libraries: {
        ToshimonUtils: utils.address,
        ...libs
      }
    });
  }

  // finally deploy all the move, item and status condition contracts
  // and add these to the registry. This uses the filename prefix as the 
  // registry index. e.g. <XXX>_<ContractName>.sol where XXX is a three digit index with leading zeroes (eg. 003 or 124)
  

  // status conditions are both libraries and contracts
  // For this reason both need to be deployed and all the library addresses collated for linking
  var libs = {};
  const statusConditionFiles = getMatchingArtifacts("../contracts/Toshimon/statusConditions/*.sol");
  let statusConditions = [];
  for (let i = 0; i < statusConditionFiles.length; i++) {
    const [index, name] = statusConditionFiles[i];

    const lib = await deployLibraryUser(name+"Lib")

    libs[name+"Lib"] = lib.address;

    const c = await deployLibraryUser(name, libs);
    statusConditions.push({ id: index, name, address: c.address })
  }
  await saveDotFile('.statusConditions.json', JSON.stringify(statusConditions));

  const itemFiles = getMatchingArtifacts("../contracts/Toshimon/items/*.sol");
  let items = [];
  for (let i = 0; i < itemFiles.length; i++) {
    const [index, name] = itemFiles[i];
    const c = await deployLibraryUser(name, libs);
    items.push({ id: index, name, address: c.address })
  }
  await saveDotFile('.items.json', JSON.stringify(items));

  const moveFiles = getMatchingArtifacts("../contracts/Toshimon/moves/*.sol");
  let moves = [];
  for (let i = 0; i < moveFiles.length; i++) {
    const [index, name] = moveFiles[i];
    const c = await deployLibraryUser(name, libs);
    moves.push({ id: index, name, address: c.address })
  }
  await saveDotFile('.moves.json', JSON.stringify(moves));

};


module.exports.tags = ['Toshimon'];
