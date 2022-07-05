// SPDX-License-Identifier: MIT


/**
 * Deploy all the contracts required for a Toshimon game deployment
 * This includes:
 *   - The Adjudicator
 *   - The ToshimonStateTransition ForceMoveApp
 *   - Every move, item and status condition contract in the respective directories (e.g contracts/Toshimon/moves/*.sol)
 *   - A Registry contract which records the locations of all the moves etc.
 * All of the deployed moves, items and conditions are also registered in the registry automatically on deployment.
 */

var glob = require("glob");
const path = require('path');

module.exports = async ({getNamedAccounts, deployments}) => {
  const {deploy, execute, log} = deployments;
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

  const registry = await deploy('ToshimonRegistry', {
    from: deployer,
    args: [],
    log: true,
  });

  await deploy('ToshimonStateTransition', {
    from: deployer,
    args: [registry.address],
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

  async function deployAndAddToRegistry(index, contractName, registryFn) {
    let r = await deploy(contractName, {
      from: deployer,
      args: [],
      log: true,
      libraries: {
        ToshimonUtils: utils.address
      }
    });
    // add it to the registry
    await execute(
      'ToshimonRegistry', {
        from: deployer
      },
      registryFn,
      index,
      r.address
    );
  }

  // finally deploy all the move, item and status condition contracts
  // and add these to the registry. This uses the filename prefix as the 
  // registry index. e.g. <XXX>_<ContractName>.sol where XXX is a three digit index with leading zeroes (eg. 003 or 124)
  
  const itemFiles = getMatchingArtifacts("../contracts/Toshimon/items/*.sol");
  for (let i = 0; i < itemFiles.length; i++) {
    const [index, name] = itemFiles[i];
    await deployAndAddToRegistry(index, name, 'addItem');
  }

  const moveFiles = getMatchingArtifacts("../contracts/Toshimon/moves/*.sol");
  for (let i = 0; i < moveFiles.length; i++) {
    const [index, name] = moveFiles[i];
    await deployAndAddToRegistry(index, name, 'addMove');
  }

};


module.exports.tags = ['Toshimon'];
