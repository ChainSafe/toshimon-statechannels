// SPDX-License-Identifier: MIT
pragma solidity 0.7.6;

interface IStatusManager {
    enum ChannelMode {
        Open,
        Challenge,
        Finalized
    }

    struct ChannelData {
        uint48 turnNumRecord;
        uint48 finalizesAt;
        bytes32 stateHash; // keccak256(abi.encode(State))
        bytes32 outcomeHash;
    }
}
