// SPDX-License-Identifier: MIT
pragma solidity 0.7.6;
pragma experimental ABIEncoderV2;

import './interfaces/IForceMoveApp.sol';

/**
 * @dev The Trivialp contracts complies with the ForceMoveApp interface and allows all transitions, regardless of the data. Used for testing purposes.
 */
contract TrivialApp is IForceMoveApp {
    /**
     * @notice Encodes trivial rules.
     * @dev Encodes trivial rules.
     * @return true.
     */
    function validTransition(
        VariablePart memory, // a
        VariablePart memory, // b
        uint256 // nParticipants
    ) public pure override returns (bool) {
        return true;
    }
}
