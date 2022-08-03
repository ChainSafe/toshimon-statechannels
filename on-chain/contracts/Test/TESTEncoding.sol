// SPDX-License-Identifier: UNLICENSED
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import '@statechannels/nitro-protocol/contracts/interfaces/INitroTypes.sol';
import { NitroUtils } from '@statechannels/nitro-protocol/contracts/libraries/NitroUtils.sol';
import { ToshimonState as TM } from '../Toshimon/ToshimonState.sol';
import { CommitRevealApp as CM } from '../CommitReveal/CommitRevealApp.sol';

contract TESTEncoding {
    function encodeAppData(
        CM.AppData memory payload
    ) public pure returns (bytes memory) {
        return (abi.encode(payload));
    }

    function encodeState(
        TM.GameState memory payload
    ) public pure returns (bytes memory) {
        return (abi.encode(payload));
    }

    function encodePlayerState(
        TM.PlayerState memory payload
    ) public pure returns (bytes memory) {
        return (abi.encode(payload));
    }

    function encodeStats(
        TM.Stats memory payload
    ) public pure returns (bytes memory) {
        return (abi.encode(payload));
    }

    function encodeItemCard(
        TM.ItemCard memory payload
    ) public pure returns (bytes memory) {
        return (abi.encode(payload));
    }

    function encodeMonsterCard(
        TM.MonsterCard memory payload
    ) public pure returns (bytes memory) {
        return (abi.encode(payload));
    }
}
