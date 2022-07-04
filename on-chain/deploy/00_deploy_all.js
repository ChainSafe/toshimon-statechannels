// Deploy all the contract required for a Toshimon instance

module.exports = async ({getNamedAccounts, deployments}) => {
  const {deploy} = deployments;
  const {deployer} = await getNamedAccounts();

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

  // finally deploy all the move, item and status condition contracts
  await deploy('Restore', {
      from: deployer,
      args: [],
      log: true,
    });
};

module.exports.tags = ['Toshimon'];
