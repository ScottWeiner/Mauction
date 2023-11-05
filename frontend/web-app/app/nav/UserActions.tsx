'use client'

import { Button, Dropdown } from 'flowbite-react'

import { User } from 'next-auth'
import { signOut } from 'next-auth/react'
import Link from 'next/link'
import React from 'react'
import { AiFillCar, AiFillTrophy, AiOutlineLoading } from 'react-icons/ai'
import { HiCog, HiUser } from 'react-icons/hi2'


type Props = {
    user: Partial<User>
}


export default function UserActions({ user }: Props) {
    return (
        <Dropdown label={`Welcome, ${user.name}`} inline>
            <Dropdown.Item icon={HiUser}>
                <Link href='/'>My Acutions</Link>
            </Dropdown.Item>
            <Dropdown.Item icon={AiFillTrophy}>
                <Link href='/'>Auctions Won</Link>
            </Dropdown.Item>
            <Dropdown.Item icon={AiFillCar}>
                <Link href='/'>Sell an Item</Link>
            </Dropdown.Item>
            <Dropdown.Item icon={HiCog}>
                <Link href='/session'>Session (Dev Only)</Link>
            </Dropdown.Item>
            <Dropdown.Divider />
            <Dropdown.Item icon={AiOutlineLoading} onClick={() => signOut({ callbackUrl: '/' })}>Sign out</Dropdown.Item>
        </Dropdown>
    )
}
