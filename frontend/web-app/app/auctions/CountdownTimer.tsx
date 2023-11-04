'use client'

import React from 'react';
import Countdown, { zeroPad } from 'react-countdown';

type Props = {
    auctionEnd: string;
}

const renderer = ({ days, hours, seconds, minutes, completed }:
    { days: number, hours: number, minutes: number, seconds: number, completed: boolean }) => {

    return (
        <div className={`
                border-2 border-white thext-while py-1 px-2 rounded-lg flex justify-center
                ${completed ? 'bg-red-600' : (days === 0 && hours < 10) ? 'bg-amber-600' : 'bg-green-600'}
            `}>
            {completed ? (
                <span>Auction finished</span>
            ) : (
                <span suppressHydrationWarning={true}>{zeroPad(days)}:{zeroPad(hours)}:{zeroPad(minutes)}:{zeroPad(seconds)}</span>
            )}
        </div>
    )

}

function CountdownTimer(props: Props) {


    return (
        <div>
            <Countdown date={props.auctionEnd} renderer={renderer} />

        </div>
    );
}

export default CountdownTimer;