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

  // finally deploy all the move contracts
};

module.exports.tags = ['Toshimon'];
