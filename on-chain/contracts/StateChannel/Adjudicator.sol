// SPDX-License-Identifier: UNLICENSED
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import '@statechannels/nitro-protocol/contracts/NitroAdjudicator.sol';
import './NFTAssetHolder.sol';
/**
 * @dev The Adjudicator contract extends MultiAssetHolder and ForceMove
 */
contract Adjudicator is NitroAdjudicator, NFTAssetHolder {

    constructor(address collection)
         NFTAssetHolder(collection) {
    }
}
